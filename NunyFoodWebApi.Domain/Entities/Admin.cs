using NunyFoodWebApi.Domain.Common;

namespace NunyFoodWebApi.Domain.Entities;

public class Admin : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
