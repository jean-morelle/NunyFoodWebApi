using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.DeliveryAgents;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.DeliveryAgents.Commands;

public record CreateDeliveryAgentCommand(
    string FullName,
    string Email,
    string Password,
    string PhoneNumber,
    string Zone) : ICommand<DeliveryAgentDto>;

public class CreateDeliveryAgentCommandValidator : AbstractValidator<CreateDeliveryAgentCommand>
{
    public CreateDeliveryAgentCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Zone).NotEmpty().MaximumLength(100);
    }
}

public class CreateDeliveryAgentCommandHandler(
    IRepository<DeliveryAgent> repository,
    IPasswordHasher passwordHasher,
    IMapper mapper) : IRequestHandler<CreateDeliveryAgentCommand, DeliveryAgentDto>
{
    public async Task<DeliveryAgentDto> Handle(CreateDeliveryAgentCommand request, CancellationToken ct)
    {
        var a = new DeliveryAgent
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            PhoneNumber = request.PhoneNumber,
            Zone = request.Zone,
            IsActive = true
        };
        repository.Add(a);
        await repository.SaveChangesAsync(ct);
        return mapper.Map<DeliveryAgentDto>(a);
    }
}
