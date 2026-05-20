using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface ITokenRepository
{
  Task<RefreshToken> CreateToken(RefreshToken token);
  Task<RefreshToken?> GetTokenByHash(string tokenHash);
  Task SaveToken(RefreshToken token);
  Task RevokeAllTokens(string userId, DateTime revokedAtUtc);
}
