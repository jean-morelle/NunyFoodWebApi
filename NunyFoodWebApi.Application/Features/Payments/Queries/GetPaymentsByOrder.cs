using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Payments;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Payments.Queries;

/// <summary>Retourne null si la commande n'existe pas ou appartient à un autre client.</summary>
public record GetPaymentsByOrderQuery(Guid OrderId) : IQuery<IEnumerable<PaymentDto>?>;

public class GetPaymentsByOrderQueryHandler(
    IRepository<Payment> paymentRepo,
    IRepository<Order> orderRepo,
    ICurrentUser currentUser,
    IMapper mapper) : IRequestHandler<GetPaymentsByOrderQuery, IEnumerable<PaymentDto>?>
{
    public async Task<IEnumerable<PaymentDto>?> Handle(GetPaymentsByOrderQuery request, CancellationToken ct)
    {
        var order = await orderRepo.GetByIdAsync(request.OrderId, ct);
        if (order is null || !currentUser.CanAccessCustomer(order.CustomerId)) return null;

        var list = await paymentRepo.FindAsync(p => p.OrderId == request.OrderId, ct);
        return mapper.Map<IEnumerable<PaymentDto>>(list);
    }
}
