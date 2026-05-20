namespace SwgohApi.Services;

public record GeneratedTokenPair(string AccessToken,
  DateTime AccessTokenExpiresAtUtc,
  string RefreshToken,
  string RefreshTokenHash,
  DateTime RefreshTokenExpiresAtUtc);
