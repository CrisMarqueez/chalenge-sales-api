using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ambev.DeveloperEvaluation.Common.Security;

/// <summary>
/// Implementation of JWT (JSON Web Token) generator.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the JWT token generator.
    /// </summary>
    /// <param name="configuration">Application configuration containing the necessary keys for token generation.</param>
    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generates a JWT token for a specific user.
    /// </summary>
    /// <param name="user">User for whom the token will be generated.</param>
    /// <returns>Valid JWT token as string.</returns>
    /// <remarks>
    /// Claims incluem:
    /// - NameIdentifier (User ID)
    /// - Name (Username)
    /// - Email
    /// - Role como dígito (0–3), alinhado à serialização JSON numérica do domínio (ex.: Admin = 3).
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when user or secret key is not provided.</exception>
    public string GenerateToken(IUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, RoleClaimValue(user.Role)),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Valor da claim de perfil: inteiro 0–3 (None, Customer, Manager, Admin), compatível com a API JSON.
    /// Aceita <paramref name="role"/> já numérico ou nome do enum.
    /// </summary>
    private static string RoleClaimValue(string role)
    {
        var raw = role?.Trim() ?? "";
        if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) && n is >= 0 and <= 3)
            return n.ToString(CultureInfo.InvariantCulture);

        if (raw.Equals("Admin", StringComparison.OrdinalIgnoreCase)) return "3";
        if (raw.Equals("Manager", StringComparison.OrdinalIgnoreCase)) return "2";
        if (raw.Equals("Customer", StringComparison.OrdinalIgnoreCase)) return "1";
        if (raw.Equals("None", StringComparison.OrdinalIgnoreCase)) return "0";
        return "0";
    }
}