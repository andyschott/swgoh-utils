namespace SwgohApi.Models.Earnables;

public record Ship(string Id,
  string Name,
  EarnableLocation[] Locations,
  Marquee? Marquee)
  : Earnable(Id, Name, Locations, Marquee);
