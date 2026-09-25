using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.DeliveryAgents;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;

namespace NunyFoodWebApi.Application.Features.DeliveryAgents.Commands;

public record UpdateDeliveryAgentCommand(
    [property: JsonIgnore] Guid Id,
    string? FullName,
    string? PhoneNumber,
    string? Zone,
    bool? IsActive) : ICommand<DeliveryAgentDto?>;

public class UpdateDeliveryAgentCommandValidator : AbstractValidator<UpdateDeliveryAgentCommand>
{
    public UpdateDeliveryAgentCommandValidator()
    {
        When(x => x.FullName is not null, () => RuleFor(x => x.FullName).NotEmpty().MaximumLength(200));
        When(x => x.PhoneNumber is not null, () => RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20));
        When(x => x.Zone is not null, () => RuleFor(x => x.Zone).NotEmpty().MaximumLength(100));
    }
}

public class UpdateDeliveryAgentCommandHandler(IRepository<DeliveryAgent> repository, IMapper mapper)
    : IRequestHandler<UpdateDeliveryAgentCommand, DeliveryAgentDto?>
{
    public async Task<DeliveryAgentDto?> Handle(UpdateDeliveryAgentCommand request, CancellationToken ct)
    {
        var a = await repository.GetByIdAsync(request.Id, ct);
        if (a is null) return null;

        if (request.FullName is not null) a.FullName = request.FullName;
        if (request.PhoneNumber is not null) a.PhoneNumber = request.PhoneNumber;
        if (request.Zone is not null) a.Zone = request.Zone;
        if (request.IsActive is not null) a.IsActive = request.IsActive.Value;

        await repository.SaveChangesAsync(ct);
        return mapper.Map<DeliveryAgentDto>(a);
    }
}
