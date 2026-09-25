using NunyFoodWebApi.Domain.Common;
using NunyFoodWebApi.Domain.Enums;

namespace NunyFoodWebApi.Domain.Entities;

public class OtpCode : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string CodeHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public OtpPurpose Purpose { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int FailedAttempts { get; set; }
}
