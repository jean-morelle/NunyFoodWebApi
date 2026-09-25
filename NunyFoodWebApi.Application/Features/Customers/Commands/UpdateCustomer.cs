using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Customers;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.Customers.Commands;

/// <summary>Retourne null si le client n'existe pas ou n'est pas accessible à l'utilisateur courant.</summary>
public record UpdateCustomerCommand(
    [property: JsonIgnore] Guid Id,
    string? FirstName,
    string? LastName,
    string? PhoneNumber) : ICommand<CustomerDto?>;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        When(x => x.FirstName is not null, () => RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100));
        When(x => x.LastName is not null, () => RuleFor(x => x.LastName).NotEmpty().MaximumLength(100));
        When(x => x.PhoneNumber is not null, () => RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20));
    }
}

public class UpdateCustomerCommandHandler(IRepository<Customer> repository, ICurrentUser currentUser, IMapper mapper)
    : IRequestHandler<UpdateCustomerCommand, CustomerDto?>
{
    public async Task<CustomerDto?> Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        if (!currentUser.CanAccessCustomer(request.Id)) return null;

        var c = await repository.GetByIdAsync(request.Id, ct);
        if (c is null) return null;

        if (request.FirstName is not null) c.FirstName = request.FirstName;
        if (request.LastName is not null) c.LastName = request.LastName;
        if (request.PhoneNumber is not null) c.PhoneNumber = request.PhoneNumber;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<CustomerDto>(c);
    }
}
