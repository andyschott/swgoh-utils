using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoFixture;

namespace SwgohApi.Tests.Extensions;

public static class FixtureExtensions
{
  extension(IFixture fixture)
  {
    public IReadOnlyDictionary<string, Claim> CreateClaims(
      string? id = null,
      string? email = null)
    {
      var claims = new Dictionary<string, Claim>();

      if (id is not null)
      {
        claims.Add(JwtRegisteredClaimNames.Sub,
          new Claim(JwtRegisteredClaimNames.Sub, id));
      }

      if (email is not null)
      {
        claims.Add(JwtRegisteredClaimNames.Email,
          new Claim(JwtRegisteredClaimNames.Email, email));
      }

      return claims;
    }
  }
}
