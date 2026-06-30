using NunyFoodWebApi.Application.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;

namespace NunyFoodWebApi.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCryptNet.HashPassword(password);
    public bool Verify(string password, string hash) => BCryptNet.Verify(password, hash);
}
