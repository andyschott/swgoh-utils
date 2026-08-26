using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Models.Auth;

namespace SwgohApi.Services;

public class AuthService : IAuthService
{
  private readonly IUserRepository _userRepository;
  private readonly ITokenRepository _tokenRepository;
  private readonly IPasswordHasher<User> _passwordHasher;
  private readonly ITokenService _tokenService;
  private readonly TimeProvider _timeProvider;

  public AuthService(IUserRepository userRepository,
    ITokenRepository tokenRepository,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService,
    TimeProvider timeProvider)
  {
    _userRepository = userRepository;
    _tokenRepository = tokenRepository;
    _passwordHasher = passwordHasher;
    _tokenService = tokenService;
    _timeProvider = timeProvider;
  }

  public async Task<ClaimsPrincipal?> Login(string email, string password)
  {
    var user = await _userRepository.GetUserByEmail(email);
    if (user is null)
    {
      return null;
    }

    var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
    if (result is not (PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded))
    {
      return null;
    }

    var claims = CreateClaims(user);
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    return new ClaimsPrincipal(identity);
  }

  public async Task<TokenResponse?> Login(LoginRequest request)
  {
    var user = await _userRepository.GetUserByEmail(request.Email);
    if (user is null)
    {
      return null;
    }

    var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
    if (result is not (PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded))
    {
      return null;
    }

    return await IssueTokenPair(user);
  }

  public async Task<TokenResponse?> Refresh(RefreshRequest request)
  {
    var refreshTokenHash = _tokenService.HashToken(request.RefreshToken);
    var refreshToken = await _tokenRepository.GetTokenByHash(refreshTokenHash);
    var now = _timeProvider.GetUtcNow().UtcDateTime;

    if (refreshToken is null ||
        refreshToken.RevokedAtUtc is not null ||
        refreshToken.ExpiresAtUtc <= now)
    {
      return null;
    }

    var user = await _userRepository.GetUserById(refreshToken.UserId);
    if (user is null)
    {
      return null;
    }

    var nextTokenResponse = await IssueTokenPair(user,
      refreshToken.Id,
      refreshToken);

    return nextTokenResponse;
  }

  public async Task Revoke(RevokeRequest request)
  {
    var refreshTokenHash = _tokenService.HashToken(request.RefreshToken);
    var refreshToken = await _tokenRepository.GetTokenByHash(refreshTokenHash);
    if (refreshToken is null || refreshToken.RevokedAtUtc is not null)
    {
      return;
    }

    refreshToken.RevokedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
    await _tokenRepository.SaveToken(refreshToken);
  }

  public async Task RevokeAll(string userId)
  {
    var now = _timeProvider.GetUtcNow().UtcDateTime;
    await _tokenRepository.RevokeAllTokens(userId, now);
  }

  private static IEnumerable<Claim> CreateClaims(User user)
  {
    return
    [
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.Email, user.Email)
    ];
  }

  private async Task<TokenResponse> IssueTokenPair(User user,
    string? parentTokenId = null,
    RefreshToken? replacedToken = null)
  {
    var claims = CreateClaims(user);
    var generatedTokenPair = _tokenService.GenerateTokenPair(claims);
    var now = _timeProvider.GetUtcNow().UtcDateTime;

    var refreshToken = new RefreshToken(Guid.NewGuid().ToString(),
      user.Id,
      generatedTokenPair.RefreshTokenHash,
      generatedTokenPair.RefreshTokenExpiresAtUtc,
      now,
      null,
      null,
      parentTokenId);

    await _tokenRepository.CreateToken(refreshToken);

    if (replacedToken is not null)
    {
      replacedToken.RevokedAtUtc = now;
      replacedToken.ReplacedByTokenId = refreshToken.Id;
      await _tokenRepository.SaveToken(replacedToken);
    }

    return new TokenResponse(generatedTokenPair.AccessToken,
      "Bearer",
      (int)(generatedTokenPair.AccessTokenExpiresAtUtc - now).TotalSeconds,
      generatedTokenPair.RefreshToken,
      (int)(generatedTokenPair.RefreshTokenExpiresAtUtc - now).TotalSeconds);
  }
}
