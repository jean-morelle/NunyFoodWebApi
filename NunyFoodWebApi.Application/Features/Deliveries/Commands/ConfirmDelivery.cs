using AutoMapper;
using FluentValidation;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.DTOs.Deliveries;
using NunyFoodWebApi.Application.Features.Orders;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Features.Deliveries.Commands;

/// <summary>
/// Le livreur confirme la remise avec une photo et la signature du receveur.
/// Retourne null si la livraison n'existe pas ou est affectée à un autre livreur.
/// </summary>
public record ConfirmDeliveryCommand(
    Guid Id,
    string ReceiverName,
    double? Latitude,
    double? Longitude,
    FileUpload? Photo,
    FileUpload? Signature) : ICommand<DeliveryDto?>;

public class ConfirmDeliveryCommandValidator : AbstractValidator<ConfirmDeliveryCommand>
{
    public ConfirmDeliveryCommandValidator()
    {
        RuleFor(x => x.ReceiverName).NotEmpty().MaximumLength(200);
        When(x => x.Latitude is not null, () => RuleFor(x => x.Latitude).InclusiveBetween(-90, 90));
        When(x => x.Longitude is not null, () => RuleFor(x => x.Longitude).InclusiveBetween(-180, 180));

        // Cascade Stop : sans fichier, on n'évalue pas BeAValidImage (qui recevrait null).
        RuleFor(x => x.Photo).Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("La photo de preuve est obligatoire.")
            .Must(BeAValidImage!).WithMessage(InvalidImageMessage);
        RuleFor(x => x.Signature).Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("La signature du receveur est obligatoire.")
            .Must(BeAValidImage!).WithMessage(InvalidImageMessage);
    }

    private static readonly string InvalidImageMessage =
        $"Image JPEG, PNG ou WebP de {ImageFiles.MaxBytes / (1024 * 1024)} Mo maximum attendue.";

    private static bool BeAValidImage(FileUpload file) =>
        file.Length is > 0 and <= ImageFiles.MaxBytes && ImageFiles.Extensions.ContainsKey(file.ContentType);
}

public class ConfirmDeliveryCommandHandler(
    IRepository<Delivery> deliveryRepo,
    IRepository<Order> orderRepo,
    IRepository<OrderStatusHistory> historyRepo,
    IFileStorage fileStorage,
    ICurrentUser currentUser,
    IMapper mapper) : IRequestHandler<ConfirmDeliveryCommand, DeliveryDto?>
{
    public async Task<DeliveryDto?> Handle(ConfirmDeliveryCommand request, CancellationToken ct)
    {
        var d = await deliveryRepo.GetByIdAsync(request.Id, ct);
        if (d is null || !currentUser.CanAccessDeliveryAgent(d.DeliveryAgentId)) return null;

        if (d.DeliveredAt is not null)
            throw new InvalidOperationException("Cette livraison a déjà été confirmée.");

        var folder = $"deliveries/{d.Id:N}";
        d.PhotoPath = await fileStorage.SaveAsync(request.Photo!, folder, ct);
        d.SignaturePath = await fileStorage.SaveAsync(request.Signature!, folder, ct);
        d.ReceiverName = request.ReceiverName;
        d.Latitude = request.Latitude;
        d.Longitude = request.Longitude;
        d.DeliveredAt = DateTime.UtcNow;

        var order = await orderRepo.GetByIdAsync(d.OrderId, ct);
        order?.ChangeStatus(OrderStatus.Delivered, historyRepo);

        await deliveryRepo.SaveChangesAsync(ct);
        return mapper.Map<DeliveryDto>(d);
    }
}
