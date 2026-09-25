using FluentValidation;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Features.Deliveries.Commands;
using NunyFoodWebApi.Application.Features.Orders.Commands;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;
using NunyFoodWebApi.Domain.Rules;
using NunyFoodWebApi.Tests.Support;

namespace NunyFoodWebApi.Tests;

public class OrderLifecycleTests
{
    private readonly TestApp _app = new();
    private readonly Customer _customer;
    private readonly Beneficiary _beneficiary;
    private readonly Pack _pack;

    public OrderLifecycleTests()
    {
        _customer = _app.AddCustomer();
        _beneficiary = _app.AddBeneficiary(_customer);
        _pack = _app.AddPack();
    }

    private Order NewOrder(OrderStatus status) => _app.AddOrder(_customer, _beneficiary, _pack, status);

    [Fact]
    public void Every_status_has_transition_rules_and_final_statuses_are_terminal()
    {
        foreach (var status in Enum.GetValues<OrderStatus>())
            _ = OrderStatusTransitions.NextStatuses(status); // lève KeyNotFoundException si un statut est oublié

        Assert.Empty(OrderStatusTransitions.NextStatuses(OrderStatus.Confirmed));
        Assert.Empty(OrderStatusTransitions.NextStatuses(OrderStatus.Cancelled));
        Assert.False(OrderStatusTransitions.CanTransition(OrderStatus.Created, OrderStatus.Delivered));
        Assert.False(OrderStatusTransitions.CanTransition(OrderStatus.InDelivery, OrderStatus.Cancelled));
    }

    [Theory]
    [InlineData(OrderStatus.Created)]
    [InlineData(OrderStatus.PendingPayment)]
    public async Task Customer_can_cancel_an_unpaid_order(OrderStatus status)
    {
        var order = NewOrder(status);
        _app.SignInAs(_customer.Id, Roles.Customer);

        var result = await _app.Send(new CancelOrderCommand(order.Id));

        Assert.Equal(OrderStatus.Cancelled, result!.Status);
        Assert.Contains(_app.Set<OrderStatusHistory>(), h => h.OrderId == order.Id && h.Status == OrderStatus.Cancelled);
    }

    [Fact]
    public async Task Customer_cannot_cancel_a_paid_order()
    {
        var order = NewOrder(OrderStatus.Paid);
        _app.SignInAs(_customer.Id, Roles.Customer);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _app.Send(new CancelOrderCommand(order.Id)));
        Assert.Equal(OrderStatus.Paid, order.Status);
    }

    [Fact]
    public async Task Customer_cannot_cancel_or_confirm_someone_elses_order()
    {
        var order = NewOrder(OrderStatus.Created);
        _app.SignInAs(_app.AddCustomer("autre@test.local").Id, Roles.Customer);

        Assert.Null(await _app.Send(new CancelOrderCommand(order.Id)));
        Assert.Null(await _app.Send(new ConfirmReceptionCommand(order.Id)));
        Assert.Equal(OrderStatus.Created, order.Status);
    }

    [Fact]
    public async Task Customer_confirms_reception_only_after_delivery()
    {
        var pending = NewOrder(OrderStatus.InDelivery);
        var delivered = NewOrder(OrderStatus.Delivered);
        _app.SignInAs(_customer.Id, Roles.Customer);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _app.Send(new ConfirmReceptionCommand(pending.Id)));
        Assert.Equal(OrderStatus.Confirmed, (await _app.Send(new ConfirmReceptionCommand(delivered.Id)))!.Status);
    }

    [Theory]
    [InlineData(OrderStatus.Paid)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Confirmed)]
    public async Task Admin_can_only_set_preparing_or_cancelled_by_hand(OrderStatus target)
    {
        var order = NewOrder(OrderStatus.Created);
        _app.SignInAsAdmin();

        await Assert.ThrowsAsync<ValidationException>(() => _app.Send(new UpdateOrderStatusCommand(order.Id, target)));
        Assert.Equal(OrderStatus.Created, order.Status);
    }

    [Fact]
    public async Task Admin_manual_change_must_follow_the_lifecycle()
    {
        var unpaid = NewOrder(OrderStatus.Created);
        var paid = NewOrder(OrderStatus.Paid);
        _app.SignInAsAdmin();

        await Assert.ThrowsAsync<InvalidOperationException>(() => _app.Send(new UpdateOrderStatusCommand(unpaid.Id, OrderStatus.Preparing)));
        Assert.Equal(OrderStatus.Preparing, (await _app.Send(new UpdateOrderStatusCommand(paid.Id, OrderStatus.Preparing)))!.Status);
    }

    [Fact]
    public async Task Unpaid_order_cannot_be_assigned_to_a_courier()
    {
        var order = NewOrder(OrderStatus.Created);
        var agent = _app.AddAgent();
        _app.SignInAsAdmin();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _app.Send(new CreateDeliveryCommand(order.Id, agent.Id, "Mama")));
        Assert.Empty(_app.Set<Delivery>());
    }

    [Fact]
    public async Task Paid_order_is_assigned_once_to_an_active_courier()
    {
        var order = NewOrder(OrderStatus.Paid);
        var inactive = _app.AddAgent("inactif@test.local");
        inactive.IsActive = false;
        var agent = _app.AddAgent();
        _app.SignInAsAdmin();

        await Assert.ThrowsAsync<ValidationException>(() => _app.Send(new CreateDeliveryCommand(order.Id, inactive.Id, "Mama")));
        await _app.Send(new CreateDeliveryCommand(order.Id, agent.Id, "Mama"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => _app.Send(new CreateDeliveryCommand(order.Id, agent.Id, "Mama")));

        Assert.Equal(OrderStatus.InDelivery, order.Status);
        Assert.Single(_app.Set<Delivery>());
    }
}
