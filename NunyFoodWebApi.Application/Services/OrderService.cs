using AutoMapper;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Services;

public class OrderService(
    IRepository<Order> orderRepo,
    IRepository<Pack> packRepo,
    IRepository<OrderStatusHistory> historyRepo,
    IMapper mapper) : IOrderService
{
    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var o = await orderRepo.GetByIdAsync(id, ct);
        return o is null ? null : mapper.Map<OrderDto>(o);
    }

    public async Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        var list = await orderRepo.FindAsync(o => o.CustomerId == customerId, ct);
        return mapper.Map<IEnumerable<OrderDto>>(list);
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await orderRepo.GetAllAsync(ct);
        return mapper.Map<IEnumerable<OrderDto>>(list);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        var pack = await packRepo.GetByIdAsync(dto.PackId, ct)
            ?? throw new KeyNotFoundException($"Pack {dto.PackId} not found.");

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            BeneficiaryId = dto.BeneficiaryId,
            PackId = dto.PackId,
            Amount = pack.Price,
            Status = OrderStatus.Created
        };

        orderRepo.Add(order);
        historyRepo.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = OrderStatus.Created,
            ChangedAt = order.CreatedAt
        });

        await orderRepo.SaveChangesAsync(ct);
        return mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto?> UpdateStatusAsync(Guid id, OrderStatus newStatus, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByIdAsync(id, ct);
        if (order is null) return null;

        order.Status = newStatus;
        historyRepo.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = newStatus,
            ChangedAt = DateTime.UtcNow
        });

        await orderRepo.SaveChangesAsync(ct);
        return mapper.Map<OrderDto>(order);
    }

    public async Task<IEnumerable<OrderStatusHistoryDto>> GetStatusHistoryAsync(Guid orderId, CancellationToken ct = default)
    {
        var history = await historyRepo.FindAsync(h => h.OrderId == orderId, ct);
        return mapper.Map<IEnumerable<OrderStatusHistoryDto>>(history.OrderBy(h => h.ChangedAt));
    }
}
