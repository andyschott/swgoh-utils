namespace SwgohApi.Auth;

public record TokenResponse(string AccessToken,
  string TokenType,
  int ExpiresIn,
  string RefreshToken);
