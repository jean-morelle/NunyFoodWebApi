using FluentValidation;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Features.Beneficiaries.Commands;
using NunyFoodWebApi.Application.Features.Beneficiaries.Queries;
using NunyFoodWebApi.Application.Features.Customers.Queries;
using NunyFoodWebApi.Application.Features.Deliveries.Queries;
using NunyFoodWebApi.Application.Features.Orders.Commands;
using NunyFoodWebApi.Application.Features.Orders.Queries;
using NunyFoodWebApi.Application.Features.Payments.Queries;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Tests.Support;

namespace NunyFoodWebApi.Tests;

/// <summary>Un client ne voit que ses données, un livreur que ses livraisons ; l'admin voit tout.</summary>
public class OwnershipTests
{
    private readonly TestApp _app = new();
    private readonly Customer _alice;
    private readonly Customer _bob;
    private readonly Beneficiary _aliceBeneficiary;
    private readonly Order _aliceOrder;

    public OwnershipTests()
    {
        _alice = _app.AddCustomer("alice@test.local");
        _bob = _app.AddCustomer("bob@test.local");
        _aliceBeneficiary = _app.AddBeneficiary(_alice);
        _aliceOrder = _app.AddOrder(_alice, _aliceBeneficiary, _app.AddPack());
    }

    [Fact]
    public async Task Customer_cannot_read_another_customers_data()
    {
        _app.SignInAs(_bob.Id, Roles.Customer);

        Assert.Null(await _app.Send(new GetOrderByIdQuery(_aliceOrder.Id)));
        Assert.Null(await _app.Send(new GetOrderStatusHistoryQuery(_aliceOrder.Id)));
        Assert.Null(await _app.Send(new GetBeneficiaryByIdQuery(_aliceBeneficiary.Id)));
        Assert.Null(await _app.Send(new GetCustomerByIdQuery(_alice.Id)));
        Assert.Null(await _app.Send(new GetPaymentsByOrderQuery(_aliceOrder.Id)));
    }

    [Fact]
    public async Task Customer_list_queries_ignore_the_customerId_parameter()
    {
        _app.SignInAs(_bob.Id, Roles.Customer);

        Assert.Empty(await _app.Send(new GetOrdersQuery(_alice.Id)));
        Assert.Empty(await _app.Send(new GetOrdersQuery(null)));
        Assert.Empty(await _app.Send(new GetBeneficiariesByCustomerQuery(_alice.Id)));
    }

    [Fact]
    public async Task Customer_cannot_modify_another_customers_beneficiary()
    {
        _app.SignInAs(_bob.Id, Roles.Customer);

        Assert.Null(await _app.Send(new UpdateBeneficiaryCommand(_aliceBeneficiary.Id, "Pirate", null, null, null)));
        Assert.False(await _app.Send(new DeleteBeneficiaryCommand(_aliceBeneficiary.Id)));
        Assert.Equal("Mama", _aliceBeneficiary.FullName);
    }

    [Fact]
    public async Task Customer_cannot_order_for_another_customers_beneficiary()
    {
        _app.SignInAs(_bob.Id, Roles.Customer);
        var pack = _app.AddPack();

        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _app.Send(new CreateOrderCommand(_aliceBeneficiary.Id, pack.Id)));

        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(CreateOrderCommand.BeneficiaryId));
    }

    [Fact]
    public async Task Order_is_always_created_for_the_signed_in_customer_at_the_pack_price()
    {
        _app.SignInAs(_alice.Id, Roles.Customer);
        var pack = _app.AddPack(price: 42000);

        var order = await _app.Send(new CreateOrderCommand(_aliceBeneficiary.Id, pack.Id));

        Assert.Equal(_alice.Id, order.CustomerId);
        Assert.Equal(42000, order.Amount);
    }

    [Fact]
    public async Task Admin_can_read_everything()
    {
        _app.SignInAsAdmin();

        Assert.NotNull(await _app.Send(new GetOrderByIdQuery(_aliceOrder.Id)));
        Assert.NotNull(await _app.Send(new GetCustomerByIdQuery(_alice.Id)));
        Assert.Single(await _app.Send(new GetOrdersQuery(null)));
    }

    [Fact]
    public async Task Agent_only_sees_own_deliveries()
    {
        var kofi = _app.AddAgent("kofi@test.local");
        var yao = _app.AddAgent("yao@test.local");
        var delivery = _app.Add(new Delivery { OrderId = _aliceOrder.Id, DeliveryAgentId = kofi.Id, ReceiverName = "Mama" });

        _app.SignInAs(yao.Id, Roles.DeliveryAgent);
        Assert.Null(await _app.Send(new GetDeliveryByIdQuery(delivery.Id)));
        Assert.Empty(await _app.Send(new GetDeliveriesByAgentQuery(kofi.Id)));

        _app.SignInAs(kofi.Id, Roles.DeliveryAgent);
        Assert.NotNull(await _app.Send(new GetDeliveryByIdQuery(delivery.Id)));
        Assert.Single(await _app.Send(new GetDeliveriesByAgentQuery(Guid.Empty)));
    }

    [Fact]
    public async Task Beneficiary_with_orders_cannot_be_deleted()
    {
        _app.SignInAs(_alice.Id, Roles.Customer);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _app.Send(new DeleteBeneficiaryCommand(_aliceBeneficiary.Id)));
        Assert.Contains(_aliceBeneficiary, _app.Set<Beneficiary>());
    }
}
