using SwgohApi.Models.Earnables;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;

namespace SwgohApi.Mapping;

public class ShipMapper : EarnableMapper<InternalShip, Ship>
{
  public ShipMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper,
    IMapper<InternalMarquee, Marquee> marqueeMapper,
    IMapper<InternalConquestReward, ConquestReward> conquestRewardMapper,
    IMapper<InternalEarnableShards, EarnableShards> earnableShardsMapper)
  : base(locationMapper, marqueeMapper, conquestRewardMapper, earnableShardsMapper)
  {
  }

  protected override Ship Create(InternalShip earnable,
    EarnableLocation[] earnableLocations,
    Marquee? marquee,
    ConquestReward? conquestReward,
    EarnableShards? earnableShardsShards)
  {
    return new Ship(earnable.Id,
      earnable.Name,
      earnableLocations,
      marquee,
      conquestReward,
      earnableShardsShards);
  }

  protected override InternalShip Create(Ship earnable, List<InternalEarnableLocation> earnableLocations)
  {
    var ship = new InternalShip
    {
      Id = earnable.Id,
      Name = earnable.Name,
      Locations = earnableLocations
    };

    return ship;
  }
}
