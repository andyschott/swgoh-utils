namespace SwgohApi.Infrastructure.Models;

public record RefreshToken
{
  public RefreshToken(string id,
    string userId,
    string tokenHash,
    DateTime expiresAtUtc,
    DateTime createdAtUtc,
    DateTime? revokedAtUtc,
    string? replacedByTokenId,
    string? parentTokenId)
  {
    Id = id;
    UserId = userId;
    TokenHash = tokenHash;
    ExpiresAtUtc = expiresAtUtc;
    CreatedAtUtc = createdAtUtc;
    RevokedAtUtc = revokedAtUtc;
    ReplacedByTokenId = replacedByTokenId;
    ParentTokenId = parentTokenId;
  }

  public string Id { get; init; }
  public string UserId { get; init; }
  public string TokenHash { get; init; }
  public DateTime ExpiresAtUtc { get; init; }
  public DateTime CreatedAtUtc { get; init; }
  public DateTime? RevokedAtUtc { get; set; }
  public string? ReplacedByTokenId { get; set; }
  public string? ParentTokenId { get; init; }
}
