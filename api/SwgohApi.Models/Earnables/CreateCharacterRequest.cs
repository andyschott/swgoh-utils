namespace SwgohApi.Models.Earnables;

public record CreateCharacterRequest(string Name,
  EarnableLocation[] Locations,
  bool IsAccelerated);
