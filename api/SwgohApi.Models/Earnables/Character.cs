namespace SwgohApi.Models.Earnables;

public record Character(string Id,
  string Name,
  EarnableLocation[] Locations,
  bool IsAccelerated)
  : Earnable(Id, Name, Locations);
