using Microsoft.AspNetCore.Identity;
using SwgohApi.Auth;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Services;

namespace SwgohApi.Tests;

public class AuthServiceTests
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);
  private readonly Mock<IUserRepository> _mockUserRepository;
  private readonly Mock<ITokenRepository> _mockTokenRepository;
  private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
  private readonly Mock<ITokenService> _mockTokenService;
  private readonly TimeProvider _timeProvider;
  private readonly AuthService _authService;

  public AuthServiceTests()
  {
    _mockUserRepository = _mockRepository.Create<IUserRepository>();
    _mockTokenRepository = _mockRepository.Create<ITokenRepository>();
    _mockPasswordHasher = _mockRepository.Create<IPasswordHasher<User>>();
    _mockTokenService = _mockRepository.Create<ITokenService>();
    _timeProvider = new FakeTimeProvider(DateTimeOffset.Parse("2026-05-20T12:00:00Z"));

    _authService = new AuthService(_mockUserRepository.Object,
      _mockTokenRepository.Object,
      _mockPasswordHasher.Object,
      _mockTokenService.Object,
      _timeProvider);
  }

  [Theory, AutoData]
  public async Task Login_ValidCredentials_ReturnsTokenResponse(User user,
    string password,
    GeneratedTokenPair generatedTokens)
  {
    var request = new LoginRequest(user.Email, password);

    _mockUserRepository.Setup(x => x.GetUserByEmail(user.Email))
      .ReturnsAsync(user);
    _mockPasswordHasher.Setup(x => x.VerifyHashedPassword(user, user.Password, password))
      .Returns(PasswordVerificationResult.Success);
    _mockTokenService.Setup(x => x.GenerateTokenPair(user))
      .Returns(generatedTokens);
    _mockTokenRepository.Setup(x => x.CreateToken(It.IsAny<RefreshToken>()))
      .ReturnsAsync((RefreshToken token) => token);

    var result = await _authService.Login(request);

    Assert.NotNull(result);
    Assert.Equal(generatedTokens.AccessToken, result.AccessToken);
    Assert.Equal(generatedTokens.RefreshToken, result.RefreshToken);
    Assert.Equal("Bearer", result.TokenType);
  }

  [Theory, AutoData]
  public async Task Login_InvalidPassword_ReturnsNull(User user,
    string password)
  {
    var request = new LoginRequest(user.Email, password);

    _mockUserRepository.Setup(x => x.GetUserByEmail(user.Email))
      .ReturnsAsync(user);
    _mockPasswordHasher.Setup(x => x.VerifyHashedPassword(user, user.Password, password))
      .Returns(PasswordVerificationResult.Failed);

    var result = await _authService.Login(request);

    Assert.Null(result);
  }

  [Theory, AutoData]
  public async Task Refresh_ValidToken_RotatesAndReturnsTokenResponse(User user,
    RefreshToken existingToken,
    GeneratedTokenPair generatedTokens,
    string refreshToken)
  {
    var request = new RefreshRequest(refreshToken);
    var refreshTokenHash = "hash";
    var now = _timeProvider.GetUtcNow().UtcDateTime;
    var activeToken = new RefreshToken(existingToken.Id,
      user.Id,
      existingToken.TokenHash,
      now.AddMinutes(10),
      now.AddMinutes(-10),
      null,
      null,
      null);

    _mockTokenService.Setup(x => x.HashToken(refreshToken))
      .Returns(refreshTokenHash);
    _mockTokenRepository.Setup(x => x.GetTokenByHash(refreshTokenHash))
      .ReturnsAsync(activeToken);
    _mockUserRepository.Setup(x => x.GetUserById(user.Id))
      .ReturnsAsync(user);
    _mockTokenService.Setup(x => x.GenerateTokenPair(user))
      .Returns(generatedTokens);
    _mockTokenRepository.Setup(x => x.CreateToken(It.IsAny<RefreshToken>()))
      .ReturnsAsync((RefreshToken token) => token);
    _mockTokenRepository.Setup(x => x.SaveToken(activeToken))
      .Returns(Task.CompletedTask);

    var result = await _authService.Refresh(request);

    Assert.NotNull(result);
    Assert.Equal(generatedTokens.AccessToken, result.AccessToken);
    Assert.Equal(generatedTokens.RefreshToken, result.RefreshToken);
    Assert.NotNull(activeToken.RevokedAtUtc);
    Assert.False(string.IsNullOrEmpty(activeToken.ReplacedByTokenId));
  }

  [Theory, AutoData]
  public async Task Refresh_RevokedToken_ReturnsNull(string refreshToken,
    RefreshToken existingToken)
  {
    var request = new RefreshRequest(refreshToken);
    var refreshTokenHash = "hash";
    var revokedToken = existingToken with
    {
      RevokedAtUtc = _timeProvider.GetUtcNow().UtcDateTime
    };

    _mockTokenService.Setup(x => x.HashToken(refreshToken))
      .Returns(refreshTokenHash);
    _mockTokenRepository.Setup(x => x.GetTokenByHash(refreshTokenHash))
      .ReturnsAsync(revokedToken);

    var result = await _authService.Refresh(request);

    Assert.Null(result);
  }

  [Theory, AutoData]
  public async Task RevokeAll_Successful(string userId)
  {
    var revokedAt = _timeProvider.GetUtcNow().UtcDateTime;
    _mockTokenRepository.Setup(x => x.RevokeAllTokens(userId, revokedAt))
      .Returns(Task.CompletedTask);

    await _authService.RevokeAll(userId);
  }
}

internal sealed class FakeTimeProvider : TimeProvider
{
  private readonly DateTimeOffset _utcNow;

  public FakeTimeProvider(DateTimeOffset utcNow)
  {
    _utcNow = utcNow;
  }

  public override DateTimeOffset GetUtcNow() => _utcNow;
}
