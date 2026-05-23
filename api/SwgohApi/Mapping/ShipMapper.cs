using SwgohApi.Models.Earnables;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Mapping;

public class ShipMapper : EarnableMapper<InternalShip, Ship>
{
  public ShipMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper)
  : base(locationMapper)
  {
  }

  protected override Ship Create(InternalShip earnable, EarnableLocation[] earnableLocations)
  {
    return new Ship(earnable.Id,
      earnable.Name,
      earnableLocations);
  }

  protected override InternalShip Create(Ship earnable, List<InternalEarnableLocation> earnableLocations)
  {
    // TODO: Map marquee eventually
    return new InternalShip(earnable.Id,
      earnable.Name,
      earnableLocations,
      null);
  }
}
