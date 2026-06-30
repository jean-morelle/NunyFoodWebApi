using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Application.Mapping;
using NunyFoodWebApi.Application.Services;

namespace NunyFoodWebApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IBeneficiaryService, BeneficiaryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPackService, PackService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IDeliveryAgentService, DeliveryAgentService>();
        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<IPackProductService, PackProductService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}
