using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NunyFoodWebApi.Application;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Features.Products.Commands;
using NunyFoodWebApi.Domain.Entities;
using NunyFoodWebApi.Tests.Support;

namespace NunyFoodWebApi.Tests;

public class MessagingTests
{
    [Fact]
    public void Every_request_has_a_registered_handler()
    {
        using var app = new TestApp();
        using var scope = app.Services.CreateScope();

        var requestTypes = typeof(DependencyInjection).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                .Select(i => (Request: t, Response: i.GetGenericArguments()[0])))
            .ToList();

        Assert.NotEmpty(requestTypes);
        foreach (var (request, response) in requestTypes)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request, response);
            Assert.True(scope.ServiceProvider.GetService(handlerType) is not null, $"Aucun handler pour {request.Name}.");
        }
    }

    [Fact]
    public async Task Invalid_request_is_rejected_before_reaching_the_handler()
    {
        using var app = new TestApp();
        app.SignInAsAdmin();

        var ex = await Assert.ThrowsAsync<ValidationException>(() => app.Send(new CreateProductCommand("", "")));

        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(CreateProductCommand.Name));
        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(CreateProductCommand.Description));
        Assert.Empty(app.Set<Product>());
    }

    [Fact]
    public async Task Valid_request_returns_the_handler_result()
    {
        using var app = new TestApp();
        app.SignInAsAdmin();

        var product = await app.Send(new CreateProductCommand("Riz", "Riz parfumé 5 kg"));

        Assert.Equal("Riz", product.Name);
        Assert.True(product.IsActive);
        Assert.Single(app.Set<Product>());
    }
}
