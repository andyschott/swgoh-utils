namespace SwgohApi.Infrastructure.Models;

public class ConquestReward
{
  public string Id { get; set; } = string.Empty;

  public string? CharacterId { get; set; }
  public Character? Character { get; set; }

  public string? ShipId { get; set; }
  public Ship? Ship { get; set; }

  public ConquestRewardPhase RewardPhase { get; set; }

  public DateOnly InitialUnlockDate { get; set; }
  public DateOnly FinalRewardCreateDate { get; set; }
  public DateOnly ProvingGroundsDate { get; set; }
}
