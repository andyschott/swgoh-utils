namespace SwgohApi.Models.Earnables;

public record ConquestRewardDate(
  string Name,
  ConquestRewardPhase RewardPhase,
  DateOnly InitialUnlockDate,
  DateOnly FinalRewardCreateDate,
  DateOnly ProvingGroundsDate);
