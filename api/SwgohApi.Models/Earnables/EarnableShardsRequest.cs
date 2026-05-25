using System.ComponentModel.DataAnnotations;

namespace SwgohApi.Models.Earnables;

public record EarnableShardsRequest(
  [property: Range(0, 330)]int Shards,
  FarmingStatus FarmingStatus);
