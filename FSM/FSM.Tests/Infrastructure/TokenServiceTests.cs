using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FSM.Domain.Entities;
using FSM.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace FSM.Tests.Infrastructure;

public class TokenServiceTests
{
    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "super-secret-signing-key-with-enough-length-1234567890",
                ["JwtSettings:Issuer"] = "fsm-issuer",
                ["JwtSettings:Audience"] = "fsm-audience",
                ["JwtSettings:ExpirationInMinutes"] = "60"
            })
            .Build();

    [Fact]
    public void GenerateToken_ProducesReadableJwt_WithExpectedClaims()
    {
        var service = new TokenService(BuildConfiguration());
        var user = new AppUser { Id = 7, Email = "user@example.com", Role = "Admin" };

        var token = service.GenerateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal("fsm-issuer", jwt.Issuer);
        Assert.Contains(jwt.Audiences, a => a == "fsm-audience");
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "7");
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "user@example.com");
        Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    [Fact]
    public void GenerateToken_SetsFutureExpiry()
    {
        var service = new TokenService(BuildConfiguration());

        var token = service.GenerateToken(new AppUser { Id = 1, Email = "a@b.com", Role = "User" });

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }
}
