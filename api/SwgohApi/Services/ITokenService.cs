using System.Security.Claims;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Services;

public interface ITokenService
{
  GeneratedTokenPair GenerateTokenPair(User user);
  string HashToken(string token);
  Task<IReadOnlyDictionary<string, Claim>?> GetClaims(HttpContext httpContext);
}

