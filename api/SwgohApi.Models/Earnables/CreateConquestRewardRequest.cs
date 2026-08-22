namespace SwgohApi.Models.Earnables;

public record CreateConquestRewardRequest(string Name,
  DateOnly FirstConquestStartDate,
  bool NewRewardIsCharacter = true);
