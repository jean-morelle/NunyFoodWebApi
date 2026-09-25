using FluentValidation;
using NunyFoodWebApi.Application.Common.Messaging;

namespace NunyFoodWebApi.Application.Common.Behaviors;

/// <summary>
/// Exécute tous les validateurs FluentValidation de la requête avant son handler.
/// En cas d'erreur, lève une <see cref="ValidationException"/> que l'API transforme en 400.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));
        var failures = results.SelectMany(r => r.Errors).ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
