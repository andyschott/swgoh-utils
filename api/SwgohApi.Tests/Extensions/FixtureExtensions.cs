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
        claims.Add(ClaimTypes.NameIdentifier,
          new Claim(ClaimTypes.NameIdentifier, id));
      }

      if (email is not null)
      {
        claims.Add(ClaimTypes.Name,
          new Claim(ClaimTypes.Name, email));
      }

      return claims;
    }
  }
}
