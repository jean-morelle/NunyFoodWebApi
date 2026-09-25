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
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
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
