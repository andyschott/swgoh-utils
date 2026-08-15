namespace SwgohApi.Models.Earnables;

public record Character(string Id,
  string Name,
  EarnableLocation[] Locations,
  bool IsAccelerated,
  Marquee? Marquee,
  ConquestReward? ConquestReward,
  EarnableShards? Shards)
  : Earnable(Id, Name, Locations, Marquee, ConquestReward, Shards);
