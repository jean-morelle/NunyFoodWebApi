namespace NunyFoodWebApi.Application.Common.Messaging;

/// <summary>Message envoyé au médiateur, qui produit une réponse de type <typeparamref name="TResponse"/>.</summary>
public interface IRequest<TResponse>;

/// <summary>Opération qui modifie l'état du système (écriture).</summary>
public interface ICommand<TResponse> : IRequest<TResponse>;

/// <summary>Opération en lecture seule.</summary>
public interface IQuery<TResponse> : IRequest<TResponse>;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Étape exécutée autour de chaque handler (validation, logs, transactions...).
/// Les behaviors s'enchaînent dans l'ordre d'enregistrement, le handler est appelé en dernier.
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct);
}

public interface ISender
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default);
}
