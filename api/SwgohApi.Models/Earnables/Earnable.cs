namespace SwgohApi.Models.Earnables;

public abstract record Earnable(string Id,
  string Name,
  EarnableLocation[] Locations,
  Marquee? Marquee,
  EarnableShards? Shards);
