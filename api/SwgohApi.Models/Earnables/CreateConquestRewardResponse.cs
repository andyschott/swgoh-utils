namespace SwgohApi.Models.Earnables;

public record CreateConquestRewardResponse(ConquestRewardDate MainReward,
  ConquestRewardDate SecondaryReward,
  ConquestRewardDate ConquestShipmentReward,
  ConquestRewardDate ProvingGroundsReward);
