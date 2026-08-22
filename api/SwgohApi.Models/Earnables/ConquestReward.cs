namespace SwgohApi.Models.Earnables;

public record ConquestReward(string Id,
  ConquestRewardPhase RewardPhase,
  DateOnly InitialUnlockDate,
  DateOnly FinalRewardCreateDate,
  DateOnly ProvingGroundsDate);
