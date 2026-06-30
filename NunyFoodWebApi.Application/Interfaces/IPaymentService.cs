using NunyFoodWebApi.Application.DTOs.Payments;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<PaymentDto>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    Task<PaymentDto> CreateAsync(CreatePaymentDto dto, CancellationToken ct = default);
}
