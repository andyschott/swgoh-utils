using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Mapping;

public abstract class EarnableMapper<TInternalEarnable, TEarnable> : IMapper<TInternalEarnable, TEarnable>
where TInternalEarnable : InternalEarnable
where TEarnable : Earnable
{
  private readonly IMapper<InternalEarnableLocation, EarnableLocation> _locationMapper;
  private readonly IMapper<InternalMarquee, Marquee> _marqueeMapper;
  private readonly IMapper<InternalEarnableShards, EarnableShards> _earnableShardsMapper;

  protected EarnableMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper,
    IMapper<InternalMarquee, Marquee> marqueeMapper,
    IMapper<InternalEarnableShards, EarnableShards> earnableShardsMapper)
  {
    _locationMapper = locationMapper;
    _marqueeMapper = marqueeMapper;
    _earnableShardsMapper = earnableShardsMapper;
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

    EarnableShards? earnableShards = null;
    if (source.EarnableShards is not null)
    {
      earnableShards = _earnableShardsMapper.MapTo(source.EarnableShards);
    }

    return Create(source, locations, marquee,  earnableShards);
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

    if (destination.Shards is not null)
    {
      earnable.EarnableShards = _earnableShardsMapper.MapFrom(destination.Shards);
      if (earnable is InternalCharacter character)
      {
        earnable.EarnableShards.CharacterId = character.Id;
        earnable.EarnableShards.Character = character;

        earnable.EarnableShards.ShipId = null;
        earnable.EarnableShards.Ship = null;
      }
      else if (earnable is InternalShip ship)
      {
        earnable.EarnableShards.ShipId = ship.Id;
        earnable.EarnableShards.Ship = ship;

        earnable.EarnableShards.CharacterId = null;
        earnable.EarnableShards.Character = null;
      }
    }

    return earnable;
  }

  protected abstract TEarnable Create(TInternalEarnable earnable,
    EarnableLocation[] earnableLocations,
    Marquee? marquee,
    EarnableShards? earnableShards);
  protected abstract TInternalEarnable Create(TEarnable earnable,
    List<InternalEarnableLocation> earnableLocations);
}
