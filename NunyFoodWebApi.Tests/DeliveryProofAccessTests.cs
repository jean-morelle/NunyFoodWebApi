using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Features.Deliveries.Queries;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Domain.Enums;
using NunyFoodWebApi.Tests.Support;

namespace NunyFoodWebApi.Tests;

/// <summary>Photo et signature : visibles par l'admin, le livreur affecté et le client de la commande, personne d'autre.</summary>
public class DeliveryProofAccessTests
{
    private readonly TestApp _app = new();
    private readonly Customer _owner;
    private readonly DeliveryAgent _agent;
    private readonly Delivery _delivery;

    public DeliveryProofAccessTests()
    {
        _owner = _app.AddCustomer("proprietaire@test.local");
        var order = _app.AddOrder(_owner, _app.AddBeneficiary(_owner), _app.AddPack(), OrderStatus.Delivered);
        _agent = _app.AddAgent("affecte@test.local");

        var photo = _app.Files.SaveAsync(new FileUpload(Stream.Null, "image/png", 1), "deliveries/test").Result;
        var signature = _app.Files.SaveAsync(new FileUpload(Stream.Null, "image/png", 1), "deliveries/test").Result;
        _delivery = _app.Add(new Delivery
        {
            OrderId = order.Id, DeliveryAgentId = _agent.Id, ReceiverName = "Mama",
            PhotoPath = photo, SignaturePath = signature, DeliveredAt = DateTime.UtcNow
        });
    }

    private Task<StoredFile?> Photo() => _app.Send(new GetDeliveryProofQuery(_delivery.Id, DeliveryProofKind.Photo));
    private Task<StoredFile?> Signature() => _app.Send(new GetDeliveryProofQuery(_delivery.Id, DeliveryProofKind.Signature));

    [Fact]
    public async Task Customer_who_placed_the_order_sees_the_proof()
    {
        _app.SignInAs(_owner.Id, Roles.Customer);

        Assert.NotNull(await Photo());
        Assert.NotNull(await Signature());
        Assert.NotNull(await _app.Send(new GetDeliveryByOrderQuery(_delivery.OrderId)));
    }

    [Fact]
    public async Task Assigned_agent_and_admin_see_the_proof()
    {
        _app.SignInAs(_agent.Id, Roles.DeliveryAgent);
        Assert.NotNull(await Photo());

        _app.SignInAsAdmin();
        Assert.NotNull(await Photo());
    }

    [Fact]
    public async Task Other_customers_and_agents_see_nothing()
    {
        _app.SignInAs(_app.AddCustomer("autre@test.local").Id, Roles.Customer);
        Assert.Null(await Photo());
        Assert.Null(await Signature());
        Assert.Null(await _app.Send(new GetDeliveryByOrderQuery(_delivery.OrderId)));
        Assert.Null(await _app.Send(new GetDeliveryByIdQuery(_delivery.Id)));

        _app.SignInAs(_app.AddAgent("autre-livreur@test.local").Id, Roles.DeliveryAgent);
        Assert.Null(await Photo());
    }

    [Fact]
    public async Task Missing_proof_returns_null()
    {
        _delivery.PhotoPath = null;
        _app.SignInAsAdmin();

        Assert.Null(await Photo());
        Assert.Null(await _app.Send(new GetDeliveryProofQuery(Guid.NewGuid(), DeliveryProofKind.Photo)));
    }
}
