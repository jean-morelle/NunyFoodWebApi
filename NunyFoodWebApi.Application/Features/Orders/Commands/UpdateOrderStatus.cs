using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Orders.Commands;

public record UpdateOrderStatusCommand(
    [property: JsonIgnore] Guid Id,
    OrderStatus Status) : ICommand<OrderDto?>;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    /// <summary>
    /// Statuts que l'admin fixe à la main. Les autres découlent d'un événement réel (paiement,
    /// affectation d'un livreur, livraison confirmée, réception confirmée par le client) :
    /// les poser à la main permettrait par exemple de marquer « Payée » sans paiement.
    /// </summary>
    public static readonly OrderStatus[] ManualStatuses = [OrderStatus.Preparing, OrderStatus.Cancelled];

    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.Status).IsInEnum()
            .Must(s => ManualStatuses.Contains(s))
            .WithMessage("Seuls les statuts « En préparation » et « Annulée » se changent manuellement ; " +
                         "les autres suivent le paiement et la livraison.");
    }
}

public class UpdateOrderStatusCommandHandler(
    IRepository<Order> orderRepo,
    IRepository<OrderStatusHistory> historyRepo,
    IMapper mapper) : IRequestHandler<UpdateOrderStatusCommand, OrderDto?>
{
    public async Task<OrderDto?> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await orderRepo.GetByIdAsync(request.Id, ct);
        if (order is null) return null;

        order.ChangeStatus(request.Status, historyRepo);

        await orderRepo.SaveChangesAsync(ct);
        return mapper.Map<OrderDto>(order);
    }
}
