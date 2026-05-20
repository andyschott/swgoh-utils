using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Services;

public interface ITokenService
{
  GeneratedTokenPair GenerateTokenPair(User user);
  string HashToken(string token);
}

