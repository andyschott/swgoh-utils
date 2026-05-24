namespace SwgohApi.Models.Earnables;

public record Ship(string Id,
  string Name,
  EarnableLocation[] Locations,
  Marquee? Marquee,
  EarnableShards? Shards)
  : Earnable(Id, Name, Locations, Marquee, Shards);
