using NunyFoodWebApi.Application.DTOs.Orders;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<OrderDto>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct = default);
    Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default);
    Task<OrderDto?> UpdateStatusAsync(Guid id, OrderStatus newStatus, CancellationToken ct = default);
    Task<IEnumerable<OrderStatusHistoryDto>> GetStatusHistoryAsync(Guid orderId, CancellationToken ct = default);
}
