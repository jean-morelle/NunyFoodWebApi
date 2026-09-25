using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NunyFoodWebApi.Application.Common;
using NunyFoodWebApi.Application.Interfaces;

namespace NunyFoodWebApi.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());

    public Guid UserId =>
        Guid.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id)
            ? id
            : throw new UnauthorizedAccessException("Token sans identifiant utilisateur.");

    public bool IsCustomer => User.IsInRole(Roles.Customer);

    public bool IsDeliveryAgent => User.IsInRole(Roles.DeliveryAgent);
}
