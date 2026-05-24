using SwgohApi.Models.Earnables;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Mapping;

public class ShipMapper : EarnableMapper<InternalShip, Ship>
{
  public ShipMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper,
    IMapper<InternalMarquee, Marquee> marqueeMapper,
    IMapper<InternalEarnableShards, EarnableShards> earnableShardsMapper)
  : base(locationMapper, marqueeMapper, earnableShardsMapper)
  {
  }

  protected override Ship Create(InternalShip earnable,
    EarnableLocation[] earnableLocations,
    Marquee? marquee,
    EarnableShards? earnableShardsShards)
  {
    return new Ship(earnable.Id,
      earnable.Name,
      earnableLocations,
      marquee,
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
