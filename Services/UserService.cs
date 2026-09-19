using System.Security.Cryptography;
using System.Text;
using BryllHonorPortfolio.Models;
using Microsoft.Extensions.Options;

namespace BryllHonorPortfolio.Services;

public static class PasswordHasher
{
    public static string Hash(string password, int iterations = 100_000, int saltSize = 16, int hashSize = 32)
    {
        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256, hashSize);
        return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string stored)
    {
        var parts = stored.Split('.', 3);
        if (parts.Length != 3) return false;
        if (!int.TryParse(parts[0], out var iterations)) return false;

        byte[] salt, expected;
        try
        {
            salt = Convert.FromBase64String(parts[1]);
            expected = Convert.FromBase64String(parts[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        var actual = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}

public class UserService
{
    private readonly LoginOptions _options;

    public UserService(IOptions<LoginOptions> options)
    {
        _options = options.Value;
    }

    public bool Verify(string? username, string? password)
    {
        if (username is null || password is null) return false;
        var usernameMatches = username == _options.Username;
        var passwordMatches = PasswordHasher.Verify(password, _options.PasswordHash);
        return usernameMatches && passwordMatches;
    }
}
