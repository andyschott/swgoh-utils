namespace SwgohApi.Models.Auth;

public record TokenResponse(string AccessToken,
  string TokenType,
  int ExpiresIn,
  string RefreshToken,
  int RefreshTokenExpiresIn);
