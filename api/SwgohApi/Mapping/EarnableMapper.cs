using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;

namespace SwgohApi.Mapping;

public abstract class EarnableMapper<TInternalEarnable, TEarnable> : IMapper<TInternalEarnable, TEarnable>
where TInternalEarnable : InternalEarnable
where TEarnable : Earnable
{
  private readonly IMapper<InternalEarnableLocation, EarnableLocation> _locationMapper;
  private readonly IMapper<InternalMarquee, Marquee> _marqueeMapper;
  private readonly IMapper<InternalConquestReward, ConquestReward> _conquestRewardMapper;
  private readonly IMapper<InternalEarnableShards, EarnableShards> _earnableShardsMapper;

  protected EarnableMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper,
    IMapper<InternalMarquee, Marquee> marqueeMapper,
    IMapper<InternalConquestReward, ConquestReward> conquestRewardMapper,
    IMapper<InternalEarnableShards, EarnableShards> earnableShardsMapper)
  {
    _locationMapper = locationMapper;
    _marqueeMapper = marqueeMapper;
    _conquestRewardMapper = conquestRewardMapper;
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

    ConquestReward? conquestReward = null;
    if (source.ConquestReward is not null)
    {
      conquestReward = _conquestRewardMapper.MapTo(source.ConquestReward);
    }

    EarnableShards? earnableShards = null;
    var internalEarnableShards = source.CurrentEarnableShards;
    if (internalEarnableShards is not null)
    {
      earnableShards = _earnableShardsMapper.MapTo(internalEarnableShards);
    }

    return Create(source, locations, marquee, conquestReward, earnableShards);
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

    if (destination.ConquestReward is not null)
    {
      earnable.ConquestReward = _conquestRewardMapper.MapFrom(destination.ConquestReward);
      if (earnable is InternalCharacter character)
      {
        earnable.ConquestReward.CharacterId = character.Id;
        earnable.ConquestReward.Character = character;

        earnable.ConquestReward.ShipId = null;
        earnable.ConquestReward.Ship = null;
      }
      else if (earnable is InternalShip ship)
      {
        earnable.ConquestReward.ShipId = ship.Id;
        earnable.ConquestReward.Ship = ship;

        earnable.ConquestReward.CharacterId = null;
        earnable.ConquestReward.Character = null;
      }
    }

    if (destination.Shards is not null)
    {
      var internalEarnableShards = _earnableShardsMapper.MapFrom(destination.Shards);
      if (earnable is InternalCharacter character)
      {
        internalEarnableShards.CharacterId = character.Id;
        internalEarnableShards.Character = character;

        internalEarnableShards.ShipId = null;
        internalEarnableShards.Ship = null;
      }
      else if (earnable is InternalShip ship)
      {
        internalEarnableShards.ShipId = ship.Id;
        internalEarnableShards.Ship = ship;

        internalEarnableShards.CharacterId = null;
        internalEarnableShards.Character = null;
      }

      earnable.EarnableShards.Add(internalEarnableShards);
    }

    return earnable;
  }

  protected abstract TEarnable Create(TInternalEarnable earnable,
    EarnableLocation[] earnableLocations,
    Marquee? marquee,
    ConquestReward? conquestReward,
    EarnableShards? earnableShards);
  protected abstract TInternalEarnable Create(TEarnable earnable,
    List<InternalEarnableLocation> earnableLocations);
}
