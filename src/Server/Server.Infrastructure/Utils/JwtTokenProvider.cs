using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Abstractions;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Utils;

public class JwtTokenProvider(IOptions<JwtSettings> settings) : IJwtTokenProvider
{
  private readonly JwtSettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));

  public string GenerateTokenForAgent(string username, string role)
    => GenerateToken(new Dictionary<string, string>
      {
        { ClaimTypes.Name, username },
        { ClaimTypes.Role, role },
        { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() }
      });

  public string GenerateTokenForAgent(Guid agentId)
    => GenerateToken(new Dictionary<string, string>
      {
        { ClaimTypes.NameIdentifier, agentId.ToString() },
        { ClaimTypes.Role, "Agent" },
        { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() }
      });

  private string GenerateToken(Dictionary<string, string> claims)
  {
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var jwtClaims = claims.Select(kvp => new Claim(kvp.Key, kvp.Value)).ToList();
    jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

    var token = new JwtSecurityToken(
        issuer: _settings.Issuer,
        audience: _settings.Audience,
        claims: jwtClaims,
        expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
