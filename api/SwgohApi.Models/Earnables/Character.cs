namespace SwgohApi.Models.Earnables;

public record Character(string Id,
  string Name,
  EarnableLocation[] Locations,
  bool IsAccelerated,
  Marquee? Marquee,
  EarnableShards? Shards)
  : Earnable(Id, Name, Locations, Marquee, Shards);
