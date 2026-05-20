using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresTokenRepository : ITokenRepository
{
  private readonly IPostgresDbContext _dbContext;

  public PostgresTokenRepository(IPostgresDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<RefreshToken> CreateToken(RefreshToken token)
  {
    await _dbContext.RefreshTokens.AddAsync(token);
    await _dbContext.SaveChangesAsync();

    return token;
  }

  public async Task<RefreshToken?> GetTokenByHash(string tokenHash)
  {
    return await _dbContext.RefreshTokens
      .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
  }

  public async Task SaveToken(RefreshToken token)
  {
    _dbContext.RefreshTokens.Update(token);
    await _dbContext.SaveChangesAsync();
  }

  public async Task RevokeAllTokens(string userId, DateTime revokedAtUtc)
  {
    var tokens = await _dbContext.RefreshTokens
      .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > revokedAtUtc)
      .ToListAsync();

    foreach (var token in tokens)
    {
      token.RevokedAtUtc = revokedAtUtc;
    }

    if (tokens.Count > 0)
    {
      _dbContext.RefreshTokens.UpdateRange(tokens);
      await _dbContext.SaveChangesAsync();
    }
  }
}
