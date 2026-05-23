namespace SwgohApi.Models.Earnables;

public record Character(string Id,
  string Name,
  EarnableLocation[] Locations,
  bool IsAccelerated,
  Marquee? Marquee)
  : Earnable(Id, Name, Locations, Marquee);
