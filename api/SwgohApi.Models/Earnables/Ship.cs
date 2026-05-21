namespace SwgohApi.Models.Earnables;

public record Ship(string Id,
  string Name,
  EarnableLocation[] Locations)
  : Earnable(Id, Name, Locations);
