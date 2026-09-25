using AutoMapper;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Payments;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Payments.Queries;

/// <summary>Retourne null si le paiement n'existe pas ou concerne la commande d'un autre client.</summary>
public record GetPaymentByIdQuery(Guid Id) : IQuery<PaymentDto?>;

public class GetPaymentByIdQueryHandler(
    IRepository<Payment> paymentRepo,
    IRepository<Order> orderRepo,
    ICurrentUser currentUser,
    IMapper mapper) : IRequestHandler<GetPaymentByIdQuery, PaymentDto?>
{
    public async Task<PaymentDto?> Handle(GetPaymentByIdQuery request, CancellationToken ct)
    {
        var p = await paymentRepo.GetByIdAsync(request.Id, ct);
        if (p is null) return null;

        var order = await orderRepo.GetByIdAsync(p.OrderId, ct);
        return order is null || !currentUser.CanAccessCustomer(order.CustomerId) ? null : mapper.Map<PaymentDto>(p);
    }
}
