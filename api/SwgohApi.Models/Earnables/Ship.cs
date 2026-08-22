namespace SwgohApi.Models.Earnables;

public record Ship(string Id,
  string Name,
  EarnableLocation[] Locations,
  Marquee? Marquee,
  ConquestReward? ConquestReward,
  EarnableShards? Shards)
  : Earnable(Id, Name, Locations, Marquee, ConquestReward, Shards);
