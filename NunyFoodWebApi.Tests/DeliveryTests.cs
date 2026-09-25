using FluentValidation;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Features.Deliveries.Commands;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;
using NunyFoodWebApi.Tests.Support;

namespace NunyFoodWebApi.Tests;

public class DeliveryTests
{
    private readonly TestApp _app = new();
    private readonly DeliveryAgent _agent;
    private readonly Order _order;
    private readonly Delivery _delivery;

    public DeliveryTests()
    {
        var customer = _app.AddCustomer();
        _order = _app.AddOrder(customer, _app.AddBeneficiary(customer), _app.AddPack(), OrderStatus.InDelivery);
        _agent = _app.AddAgent();
        _delivery = _app.Add(new Delivery { OrderId = _order.Id, DeliveryAgentId = _agent.Id, ReceiverName = "Mama" });
        _app.SignInAs(_agent.Id, Roles.DeliveryAgent);
    }

    private static FileUpload Image(string contentType = "image/png", long length = 1024) =>
        new(new MemoryStream(new byte[length]), contentType, length);

    private ConfirmDeliveryCommand Confirm(FileUpload? photo, FileUpload? signature) =>
        new(_delivery.Id, "Mama", 6.1319, 1.2228, photo, signature);

    [Fact]
    public async Task Photo_and_signature_are_required()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() => _app.Send(Confirm(null, null)));

        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(ConfirmDeliveryCommand.Photo));
        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(ConfirmDeliveryCommand.Signature));
        Assert.Null(_delivery.DeliveredAt);
    }

    [Theory]
    [InlineData("text/html", 1024)]
    [InlineData("image/png", 6 * 1024 * 1024)]
    [InlineData("image/png", 0)]
    public async Task Non_image_or_oversized_files_are_refused(string contentType, long length)
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() =>
            _app.Send(Confirm(Image(contentType, length), Image())));

        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(ConfirmDeliveryCommand.Photo));
        Assert.Empty(_app.Files.Saved);
    }

    [Fact]
    public async Task Confirmation_stores_the_proof_and_marks_the_order_delivered()
    {
        var result = await _app.Send(Confirm(Image("image/jpeg"), Image()));

        Assert.NotNull(result);
        Assert.NotNull(result.DeliveredAt);
        Assert.Equal(6.1319, result.Latitude);
        Assert.Equal(2, _app.Files.Saved.Count);
        Assert.Equal(_app.Files.Saved[0], result.PhotoUrl);
        Assert.Equal(_app.Files.Saved[1], result.SignatureUrl);
        Assert.Equal(OrderStatus.Delivered, _order.Status);
    }

    [Fact]
    public async Task Delivery_cannot_be_confirmed_twice()
    {
        await _app.Send(Confirm(Image(), Image()));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _app.Send(Confirm(Image(), Image())));
        Assert.Equal(2, _app.Files.Saved.Count);
    }

    [Fact]
    public async Task Another_agent_cannot_confirm()
    {
        _app.SignInAs(_app.AddAgent("autre@test.local").Id, Roles.DeliveryAgent);

        Assert.Null(await _app.Send(Confirm(Image(), Image())));
        Assert.Null(_delivery.DeliveredAt);
        Assert.Empty(_app.Files.Saved);
    }
}
