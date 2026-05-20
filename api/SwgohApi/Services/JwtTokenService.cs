using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SwgohApi.Auth;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Services;

public class JwtTokenService : ITokenService
{
  private readonly JwtOptions _jwtOptions;
  private readonly TimeProvider _timeProvider;

  public JwtTokenService(IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider)
  {
    _jwtOptions = jwtOptions.Value;
    _timeProvider = timeProvider;
  }

  public GeneratedTokenPair GenerateTokenPair(User user)
  {
    var now = _timeProvider.GetUtcNow().UtcDateTime;
    var accessTokenExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes);
    var refreshTokenExpiresAtUtc = now.AddMinutes(_jwtOptions.RefreshTokenLifetimeMinutes);

    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.Email, user.Email)
    };

    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
    var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    var tokenDescriptor = new JwtSecurityToken(
      issuer: _jwtOptions.Issuer,
      audience: _jwtOptions.Audience,
      claims: claims,
      expires: accessTokenExpiresAtUtc,
      signingCredentials: credentials);

    var accessToken = new JwtSecurityTokenHandler()
      .WriteToken(tokenDescriptor);

    var refreshTokenBytes = RandomNumberGenerator.GetBytes(64);
    var refreshToken = Base64UrlEncoder.Encode(refreshTokenBytes);

    return new GeneratedTokenPair(accessToken,
      accessTokenExpiresAtUtc,
      refreshToken,
      HashToken(refreshToken),
      refreshTokenExpiresAtUtc);
  }

  public string HashToken(string token)
  {
    var tokenBytes = Encoding.UTF8.GetBytes(token);
    var hashBytes = SHA256.HashData(tokenBytes);

    return Convert.ToHexString(hashBytes);
  }
}
