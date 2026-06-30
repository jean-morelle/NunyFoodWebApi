namespace NunyFoodWebApi.Application.Common;

public enum PaymentProviderStatus { Succeeded, Failed, Pending }

public record PaymentProviderResult(
    PaymentProviderStatus Status,
    string? TransactionId,
    string? ErrorMessage);
