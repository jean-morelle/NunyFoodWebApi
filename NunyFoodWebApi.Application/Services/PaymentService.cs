using AutoMapper;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.DTOs.Payments;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Services;

public class PaymentService(
    IRepository<Payment> paymentRepo,
    IRepository<Order> orderRepo,
    IRepository<OrderStatusHistory> historyRepo,
    IPaymentProvider paymentProvider,
    IMapper mapper) : IPaymentService
{
    public async Task<PaymentDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await paymentRepo.GetByIdAsync(id, ct);
        return p is null ? null : mapper.Map<PaymentDto>(p);
    }

    public async Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        var list = await paymentRepo.FindAsync(p => p.OrderId == orderId, ct);
        return mapper.Map<IEnumerable<PaymentDto>>(list);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        var providerResult = await paymentProvider.ProcessAsync(dto.Amount, dto.Method, ct);

        var payment = new Payment
        {
            OrderId = dto.OrderId,
            Amount = dto.Amount,
            Method = dto.Method,
            Status = providerResult.Status switch
            {
                PaymentProviderStatus.Succeeded => PaymentStatus.Succeeded,
                PaymentProviderStatus.Failed => PaymentStatus.Failed,
                _ => PaymentStatus.Pending
            },
            TransactionId = providerResult.TransactionId
        };

        paymentRepo.Add(payment);

        if (payment.Status == PaymentStatus.Succeeded)
        {
            var order = await orderRepo.GetByIdAsync(dto.OrderId, ct);
            if (order is not null)
            {
                order.Status = OrderStatus.Paid;
                historyRepo.Add(new OrderStatusHistory
                {
                    OrderId = order.Id,
                    Status = OrderStatus.Paid,
                    ChangedAt = DateTime.UtcNow
                });
            }
        }

        await paymentRepo.SaveChangesAsync(ct);
        return mapper.Map<PaymentDto>(payment);
    }
}
