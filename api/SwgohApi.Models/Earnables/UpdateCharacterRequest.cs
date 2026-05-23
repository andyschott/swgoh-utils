namespace SwgohApi.Models.Earnables;

public record UpdateCharacterRequest(EarnableLocation[]? Locations,
  bool? IsAccelerated);
