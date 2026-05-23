using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Mapping;

public abstract class EarnableMapper<TInternalEarnable, TEarnable> : IMapper<TInternalEarnable, TEarnable>
where TInternalEarnable : InternalEarnable
where TEarnable : Earnable
{
  private readonly IMapper<InternalEarnableLocation, EarnableLocation> _locationMapper;
  private readonly IMapper<InternalMarquee, Marquee> _marqueeMapper;

  protected EarnableMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper,
    IMapper<InternalMarquee, Marquee> marqueeMapper)
  {
    _locationMapper = locationMapper;
    _marqueeMapper = marqueeMapper;
  }

  public TEarnable MapTo(TInternalEarnable source)
  {
    var locations = source.Locations.Select(location => _locationMapper.MapTo(location))
      .ToArray();
    Marquee? marquee = null;
    if (source.Marquee is not null)
    {
      marquee = _marqueeMapper.MapTo(source.Marquee);
    }

    return Create(source, locations, marquee);
  }

  public TInternalEarnable MapFrom(TEarnable destination)
  {
    var locations = destination.Locations.Select(location => _locationMapper.MapFrom(location))
      .ToList();
    var earnable = Create(destination, locations);

    if (destination.Marquee is not null)
    {
      earnable.Marquee = _marqueeMapper.MapFrom(destination.Marquee);
      if (earnable is InternalCharacter character)
      {
        earnable.Marquee.CharacterId = character.Id;
        earnable.Marquee.Character = character;

        earnable.Marquee.ShipId = null;
        earnable.Marquee.Ship = null;
      }
      else if (earnable is InternalShip ship)
      {
        earnable.Marquee.ShipId = ship.Id;
        earnable.Marquee.Ship = ship;

        earnable.Marquee.CharacterId = null;
        earnable.Marquee.Character = null;
      }
    }

    return earnable;
  }

  protected abstract TEarnable Create(TInternalEarnable earnable,
    EarnableLocation[] earnableLocations,
    Marquee? marquee);
  protected abstract TInternalEarnable Create(TEarnable earnable,
    List<InternalEarnableLocation> earnableLocations);
}
