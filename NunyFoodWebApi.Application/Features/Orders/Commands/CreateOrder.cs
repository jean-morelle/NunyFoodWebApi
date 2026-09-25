using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Orders.Commands;

/// <summary>Commande passée par le client connecté, pour l'un de ses bénéficiaires.</summary>
public record CreateOrderCommand(Guid BeneficiaryId, Guid PackId) : ICommand<OrderDto>;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.BeneficiaryId).NotEmpty();
        RuleFor(x => x.PackId).NotEmpty();
    }
}

public class CreateOrderCommandHandler(
    IRepository<Order> orderRepo,
    IRepository<Pack> packRepo,
    IRepository<Beneficiary> beneficiaryRepo,
    IRepository<OrderStatusHistory> historyRepo,
    ICurrentUser currentUser,
    IMapper mapper) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        var customerId = currentUser.UserId;

        var beneficiary = await beneficiaryRepo.GetByIdAsync(request.BeneficiaryId, ct);
        if (beneficiary is null || beneficiary.CustomerId != customerId)
            throw new ValidationException([new ValidationFailure(nameof(request.BeneficiaryId), "Bénéficiaire introuvable.")]);

        var pack = await packRepo.GetByIdAsync(request.PackId, ct)
            ?? throw new KeyNotFoundException($"Pack {request.PackId} not found.");

        var order = new Order
        {
            CustomerId = customerId,
            BeneficiaryId = request.BeneficiaryId,
            PackId = request.PackId,
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
}
