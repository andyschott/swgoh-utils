using System.Security.Claims;

namespace SwgohApi.Services;

public interface ITokenService
{
  GeneratedTokenPair GenerateTokenPair(IEnumerable<Claim> claims);
  string HashToken(string token);
  Task<IReadOnlyDictionary<string, Claim>?> GetClaims(HttpContext httpContext);
}

