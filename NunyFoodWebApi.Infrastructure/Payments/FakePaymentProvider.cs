using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Interfaces;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Infrastructure.Payments;

public class FakePaymentProvider : IPaymentProvider
{
    private static readonly Random _rng = new();

    public Task<PaymentProviderResult> ProcessAsync(decimal amount, PaymentMethod method, CancellationToken ct = default)
    {
        var result = _rng.Next(0, 3) switch
        {
            0 => new PaymentProviderResult(PaymentProviderStatus.Succeeded, Guid.NewGuid().ToString("N"), null),
            1 => new PaymentProviderResult(PaymentProviderStatus.Failed, null, "Payment declined by provider."),
            _ => new PaymentProviderResult(PaymentProviderStatus.Pending, Guid.NewGuid().ToString("N"), null)
        };
        return Task.FromResult(result);
    }
}
