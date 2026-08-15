namespace SwgohApi.Models.Earnables;

public record ConquestRewardRequest(ConquestRewardPhase RewardPhase,
  DateOnly InitialUnlockDate,
  DateOnly FinalRewardCreateDate,
  DateOnly ProvingGroundsDate);
