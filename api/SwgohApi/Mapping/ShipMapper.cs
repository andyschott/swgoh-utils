using SwgohApi.Models.Earnables;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Mapping;

public class ShipMapper : EarnableMapper<InternalShip, Ship>
{
  public ShipMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper,
    IMapper<InternalMarquee, Marquee> marqueeMapper)
  : base(locationMapper, marqueeMapper)
  {
  }

  protected override Ship Create(InternalShip earnable,
    EarnableLocation[] earnableLocations,
    Marquee? marquee)
  {
    return new Ship(earnable.Id,
      earnable.Name,
      earnableLocations,
      marquee);
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
