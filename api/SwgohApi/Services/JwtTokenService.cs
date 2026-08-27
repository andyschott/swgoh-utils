using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SwgohApi.Configuration;

namespace SwgohApi.Services;

public class JwtTokenService : ITokenService
{
  private readonly JwtOptions _jwtOptions;
  private readonly TimeProvider _timeProvider;
  private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

  public JwtTokenService(IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider)
  {
    _jwtOptions = jwtOptions.Value;
    _timeProvider = timeProvider;

    _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
  }

  public GeneratedTokenPair GenerateTokenPair(IEnumerable<Claim> claims)
  {
    var now = _timeProvider.GetUtcNow().UtcDateTime;
    var accessTokenExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes);
    var refreshTokenExpiresAtUtc = now.AddMinutes(_jwtOptions.RefreshTokenLifetimeMinutes);

    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
    var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    var tokenDescriptor = new JwtSecurityToken(
      issuer: _jwtOptions.Issuer,
      audience: _jwtOptions.Audience,
      claims: claims,
      expires: accessTokenExpiresAtUtc,
      signingCredentials: credentials);

    var accessToken = _jwtSecurityTokenHandler
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

  public async Task<IReadOnlyDictionary<string, Claim>?> GetClaims(HttpContext httpContext)
  {
    if (httpContext.User.Identity?.IsAuthenticated != true)
    {
      return null;
    }

    return httpContext.User.Claims.ToDictionary(claim => claim.Type);
  }
}
