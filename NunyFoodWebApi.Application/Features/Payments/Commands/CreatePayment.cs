using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Payments;
using NunyFoodWebApi.Application.Features.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Payments.Commands;

public record CreatePaymentCommand(Guid OrderId, decimal Amount, PaymentMethod Method) : ICommand<PaymentDto>;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).IsInEnum();
    }
}

public class CreatePaymentCommandHandler(
    IRepository<Payment> paymentRepo,
    IRepository<Order> orderRepo,
    IRepository<OrderStatusHistory> historyRepo,
    IPaymentProvider paymentProvider,
    ICurrentUser currentUser,
    IMapper mapper) : IRequestHandler<CreatePaymentCommand, PaymentDto>
{
    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        var order = await orderRepo.GetByIdAsync(request.OrderId, ct);
        if (order is null || !currentUser.CanAccessCustomer(order.CustomerId))
            throw new KeyNotFoundException($"Order {request.OrderId} not found.");

        if (order.Status is not (OrderStatus.Created or OrderStatus.PendingPayment))
            throw new InvalidOperationException("Cette commande ne peut plus être payée.");

        // Le montant fait foi côté serveur : on ne laisse pas le client payer moins que le prix du pack.
        if (request.Amount != order.Amount)
            throw new InvalidOperationException($"Le montant doit être égal au montant de la commande ({order.Amount}).");

        var providerResult = await paymentProvider.ProcessAsync(order.Amount, request.Method, ct);

        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.Amount,
            Method = request.Method,
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
            order.ChangeStatus(OrderStatus.Paid, historyRepo);

        await paymentRepo.SaveChangesAsync(ct);
        return mapper.Map<PaymentDto>(payment);
    }
}
