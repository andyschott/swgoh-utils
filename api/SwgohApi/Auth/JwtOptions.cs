using System.ComponentModel.DataAnnotations;

namespace SwgohApi.Auth;

public class JwtOptions
{
  [Required]
  public string Issuer { get; init; } = string.Empty;

  [Required]
  public string Audience { get; init; } = string.Empty;

  [Required]
  [MinLength(32)]
  public string SigningKey { get; init; } = string.Empty;

  [Range(1, 1440)]
  public int AccessTokenLifetimeMinutes { get; init; } = 60;

  [Range(1, 43200)]
  public int RefreshTokenLifetimeMinutes { get; init; } = 10080;
}
