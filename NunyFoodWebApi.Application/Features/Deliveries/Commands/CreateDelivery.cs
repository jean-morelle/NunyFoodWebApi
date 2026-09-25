using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Deliveries;
using NunyFoodWebApi.Application.Features.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Deliveries.Commands;

/// <summary>Affecte une commande à un livreur et la passe en livraison.</summary>
public record CreateDeliveryCommand(Guid OrderId, Guid DeliveryAgentId, string ReceiverName) : ICommand<DeliveryDto>;

public class CreateDeliveryCommandValidator : AbstractValidator<CreateDeliveryCommand>
{
    public CreateDeliveryCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.DeliveryAgentId).NotEmpty();
        RuleFor(x => x.ReceiverName).NotEmpty().MaximumLength(200);
    }
}

public class CreateDeliveryCommandHandler(
    IRepository<Delivery> deliveryRepo,
    IRepository<Order> orderRepo,
    IRepository<OrderStatusHistory> historyRepo,
    IMapper mapper) : IRequestHandler<CreateDeliveryCommand, DeliveryDto>
{
    public async Task<DeliveryDto> Handle(CreateDeliveryCommand request, CancellationToken ct)
    {
        var order = await orderRepo.GetByIdAsync(request.OrderId, ct)
            ?? throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        var delivery = new Delivery
        {
            OrderId = request.OrderId,
            DeliveryAgentId = request.DeliveryAgentId,
            ReceiverName = request.ReceiverName
        };

        deliveryRepo.Add(delivery);
        order.ChangeStatus(OrderStatus.InDelivery, historyRepo);
        await deliveryRepo.SaveChangesAsync(ct);
        return mapper.Map<DeliveryDto>(delivery);
    }
}
