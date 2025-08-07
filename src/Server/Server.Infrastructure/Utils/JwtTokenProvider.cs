using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Abstractions;
using Server.Infrastructure.Configs;

namespace Server.Infrastructure.Utils;

public class JwtTokenProvider : IJwtTokenProvider
{
  private readonly JwtSettings _settings;

  public JwtTokenProvider(IOptions<JwtSettings> settings)
  {
    _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
  }

  public string GenerateToken(string username, string role)
  {
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
        issuer: _settings.Issuer,
        audience: _settings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
