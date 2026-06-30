using NunyFoodWebApi.Application.DTOs.DeliveryAgents;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IDeliveryAgentService
{
    Task<DeliveryAgentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<DeliveryAgentDto>> GetAllAsync(CancellationToken ct = default);
    Task<DeliveryAgentDto> CreateAsync(CreateDeliveryAgentDto dto, CancellationToken ct = default);
    Task<DeliveryAgentDto?> UpdateAsync(Guid id, UpdateDeliveryAgentDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
