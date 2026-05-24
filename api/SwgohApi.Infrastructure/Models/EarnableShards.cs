namespace SwgohApi.Infrastructure.Models;

public class EarnableShards
{
  public string Id { get; set; } = string.Empty;

  public string UserId { get; set; } = string.Empty;
  public User? User { get; set; }

  public string? CharacterId { get; set; }
  public Character? Character { get; set; }

  public string? ShipId { get; set; }
  public Ship? Ship { get; set; }

  public int Shards { get; set; }
  public FarmingStatus FarmingStatus { get; set; } = FarmingStatus.Backlog;
}
