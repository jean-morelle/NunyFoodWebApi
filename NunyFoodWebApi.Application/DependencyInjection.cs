using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NunyFoodWebApi.Application.Common.Behaviors;
using NunyFoodWebApi.Application.Common.Messaging;
using NunyFoodWebApi.Application.Features.Auth;
using NunyFoodWebApi.Application.Mapping;

namespace NunyFoodWebApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ISender, Sender>();
        services.AddRequestHandlers(assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<AccountLookup>();
        services.AddScoped<OtpManager>();

        return services;
    }

    /// <summary>Enregistre toutes les classes qui implémentent IRequestHandler&lt;,&gt; dans l'assembly.</summary>
    private static void AddRequestHandlers(this IServiceCollection services, Assembly assembly)
    {
        var registrations =
            from type in assembly.GetTypes()
            where type is { IsClass: true, IsAbstract: false }
            from @interface in type.GetInterfaces()
            where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
            select (@interface, type);

        foreach (var (@interface, type) in registrations)
            services.AddScoped(@interface, type);
    }
}
