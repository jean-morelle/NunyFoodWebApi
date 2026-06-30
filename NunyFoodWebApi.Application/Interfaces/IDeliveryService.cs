using NunyFoodWebApi.Application.DTOs.Deliveries;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IDeliveryService
{
    Task<DeliveryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<DeliveryDto?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task<IEnumerable<DeliveryDto>> GetByAgentIdAsync(Guid agentId, CancellationToken ct = default);
    Task<DeliveryDto> CreateAsync(CreateDeliveryDto dto, CancellationToken ct = default);
    Task<DeliveryDto?> ConfirmAsync(Guid id, ConfirmDeliveryDto dto, CancellationToken ct = default);
}
