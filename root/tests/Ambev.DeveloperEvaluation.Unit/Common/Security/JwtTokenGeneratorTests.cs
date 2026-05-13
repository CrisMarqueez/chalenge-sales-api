using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Security;

public sealed class JwtTokenGeneratorTests
{
    [Fact]
    public void GenerateToken_Payload_ContainsShortRoleKeyWithAdmin()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = new string('x', 32),
            })
            .Build();

        var gen = new JwtTokenGenerator(config);
        var user = new User
        {
            Id = Guid.Parse("043344eb-0ee4-49a6-b9ee-290462411937"),
            Username = "teste admin",
            Email = "admin@test.local",
            Password = "ignored",
            Phone = "+5511999999999",
            Role = UserRole.Admin,
            Status = UserStatus.Active,
        };

        var token = gen.GenerateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Payload.ContainsKey("role").Should().BeTrue("JWT deve expor a chave curta 'role' para o bearer validar");
        jwt.Payload["role"]!.ToString().Should().Be("3");
    }
}
