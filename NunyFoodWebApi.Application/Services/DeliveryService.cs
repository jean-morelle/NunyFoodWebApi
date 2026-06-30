using AutoMapper;
using NunyFoodWebApi.Application.DTOs.Deliveries;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Services;

public class DeliveryService(
    IRepository<Delivery> deliveryRepo,
    IRepository<Order> orderRepo,
    IRepository<OrderStatusHistory> historyRepo,
    IMapper mapper) : IDeliveryService
{
    public async Task<DeliveryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var d = await deliveryRepo.GetByIdAsync(id, ct);
        return d is null ? null : mapper.Map<DeliveryDto>(d);
    }

    public async Task<DeliveryDto?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        var d = await deliveryRepo.FirstOrDefaultAsync(d => d.OrderId == orderId, ct);
        return d is null ? null : mapper.Map<DeliveryDto>(d);
    }

    public async Task<IEnumerable<DeliveryDto>> GetByAgentIdAsync(Guid agentId, CancellationToken ct = default)
    {
        var list = await deliveryRepo.FindAsync(d => d.DeliveryAgentId == agentId, ct);
        return mapper.Map<IEnumerable<DeliveryDto>>(list);
    }

    public async Task<DeliveryDto> CreateAsync(CreateDeliveryDto dto, CancellationToken ct = default)
    {
        var delivery = new Delivery
        {
            OrderId = dto.OrderId,
            DeliveryAgentId = dto.DeliveryAgentId,
            ReceiverName = dto.ReceiverName
        };

        deliveryRepo.Add(delivery);
        await UpdateOrderStatus(dto.OrderId, OrderStatus.InDelivery, ct);
        await deliveryRepo.SaveChangesAsync(ct);
        return mapper.Map<DeliveryDto>(delivery);
    }

    public async Task<DeliveryDto?> ConfirmAsync(Guid id, ConfirmDeliveryDto dto, CancellationToken ct = default)
    {
        var d = await deliveryRepo.GetByIdAsync(id, ct);
        if (d is null) return null;

        d.ReceiverName = dto.ReceiverName;
        d.PhotoUrl = dto.PhotoUrl;
        d.SignatureUrl = dto.SignatureUrl;
        d.Latitude = dto.Latitude;
        d.Longitude = dto.Longitude;
        d.DeliveredAt = DateTime.UtcNow;

        await UpdateOrderStatus(d.OrderId, OrderStatus.Delivered, ct);
        await deliveryRepo.SaveChangesAsync(ct);
        return mapper.Map<DeliveryDto>(d);
    }

    private async Task UpdateOrderStatus(Guid orderId, OrderStatus status, CancellationToken ct)
    {
        var order = await orderRepo.GetByIdAsync(orderId, ct);
        if (order is null) return;

        order.Status = status;
        historyRepo.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = status,
            ChangedAt = DateTime.UtcNow
        });
    }
}
