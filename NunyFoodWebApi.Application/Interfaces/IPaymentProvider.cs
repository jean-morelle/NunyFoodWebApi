using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Application.Interfaces;

public interface IPaymentProvider
{
    Task<PaymentProviderResult> ProcessAsync(decimal amount, PaymentMethod method, CancellationToken ct = default);
}
