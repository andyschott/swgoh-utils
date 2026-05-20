using Moq.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresTokenRepositoryTests
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);
  private readonly Mock<IPostgresDbContext> _mockDbContext;
  private readonly PostgresTokenRepository _tokenRepository;

  public PostgresTokenRepositoryTests()
  {
    _mockDbContext = _mockRepository.Create<IPostgresDbContext>(MockBehavior.Loose);
    _tokenRepository = new PostgresTokenRepository(_mockDbContext.Object);
  }

  [Theory, AutoData]
  public async Task CreateToken_Successful(RefreshToken token)
  {
    var mockRefreshTokensDb = _mockRepository.Create<Microsoft.EntityFrameworkCore.DbSet<RefreshToken>>(MockBehavior.Loose);
    _mockDbContext.Setup(x => x.RefreshTokens)
      .Returns(mockRefreshTokensDb.Object);

    var result = await _tokenRepository.CreateToken(token);

    Assert.Same(token, result);
    _mockDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
  }

  [Theory, AutoData]
  public async Task GetTokenByHash_Successful(RefreshToken token)
  {
    _mockDbContext.Setup(x => x.RefreshTokens)
      .ReturnsDbSet([token]);

    var result = await _tokenRepository.GetTokenByHash(token.TokenHash);

    Assert.Same(token, result);
  }

  [Theory, AutoData]
  public async Task GetTokenByHash_NotFound_ReturnsNull(RefreshToken token,
    string hash)
  {
    _mockDbContext.Setup(x => x.RefreshTokens)
      .ReturnsDbSet([token]);

    var result = await _tokenRepository.GetTokenByHash(hash);

    Assert.Null(result);
  }

  [Theory, AutoData]
  public async Task RevokeAllTokens_RevokesActiveUserTokens(RefreshToken token1,
    RefreshToken token2,
    DateTime revokedAtUtc)
  {
    var activeToken = token1 with
    {
      UserId = "user-1",
      RevokedAtUtc = null,
      ExpiresAtUtc = revokedAtUtc.AddMinutes(5)
    };
    var expiredToken = token2 with
    {
      UserId = "user-1",
      RevokedAtUtc = null,
      ExpiresAtUtc = revokedAtUtc.AddMinutes(-5)
    };

    _mockDbContext.Setup(x => x.RefreshTokens)
      .ReturnsDbSet([activeToken, expiredToken]);

    await _tokenRepository.RevokeAllTokens("user-1", revokedAtUtc);

    Assert.Equal(revokedAtUtc, activeToken.RevokedAtUtc);
    Assert.Null(expiredToken.RevokedAtUtc);
    _mockDbContext.Verify(x => x.SaveChangesAsync(), Times.Once);
  }
}
