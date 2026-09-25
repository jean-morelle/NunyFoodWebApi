using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Features.Payments.Commands;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;
using NunyFoodWebApi.Tests.Support;

namespace NunyFoodWebApi.Tests;

public class PaymentTests
{
    private readonly TestApp _app = new();
    private readonly Customer _customer;
    private readonly Order _order;

    public PaymentTests()
    {
        _customer = _app.AddCustomer();
        _order = _app.AddOrder(_customer, _app.AddBeneficiary(_customer), _app.AddPack(price: 15000));
        _app.SignInAs(_customer.Id, Roles.Customer);
    }

    [Fact]
    public async Task Successful_payment_marks_the_order_paid_and_records_history()
    {
        var payment = await _app.Send(new CreatePaymentCommand(_order.Id, 15000, PaymentMethod.TMoney));

        Assert.Equal(PaymentStatus.Succeeded, payment.Status);
        Assert.Equal(OrderStatus.Paid, _order.Status);
        Assert.Contains(_app.Set<OrderStatusHistory>(), h => h.OrderId == _order.Id && h.Status == OrderStatus.Paid);
    }

    [Fact]
    public async Task Amount_different_from_the_order_is_refused()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _app.Send(new CreatePaymentCommand(_order.Id, 1, PaymentMethod.TMoney)));

        Assert.Empty(_app.Set<Payment>());
        Assert.Equal(OrderStatus.Created, _order.Status);
    }

    [Fact]
    public async Task Paid_order_cannot_be_paid_again()
    {
        await _app.Send(new CreatePaymentCommand(_order.Id, 15000, PaymentMethod.Flooz));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _app.Send(new CreatePaymentCommand(_order.Id, 15000, PaymentMethod.Flooz)));
        Assert.Single(_app.Set<Payment>());
    }

    [Fact]
    public async Task Failed_payment_leaves_the_order_unpaid()
    {
        _app.Payments.NextStatus = PaymentProviderStatus.Failed;

        var payment = await _app.Send(new CreatePaymentCommand(_order.Id, 15000, PaymentMethod.PayPal));

        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal(OrderStatus.Created, _order.Status);
    }

    [Fact]
    public async Task Customer_cannot_pay_another_customers_order()
    {
        _app.SignInAs(_app.AddCustomer("autre@test.local").Id, Roles.Customer);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _app.Send(new CreatePaymentCommand(_order.Id, 15000, PaymentMethod.TMoney)));
    }
}
