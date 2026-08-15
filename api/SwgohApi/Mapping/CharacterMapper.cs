using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;

namespace SwgohApi.Mapping;

public class CharacterMapper : EarnableMapper<InternalCharacter, Character>
{
  public CharacterMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper,
    IMapper<InternalMarquee, Marquee> marqueeMapper,
    IMapper<InternalConquestReward, ConquestReward> conquestRewardMapper,
    IMapper<InternalEarnableShards, EarnableShards> earnableShardsMapper)
  : base(locationMapper, marqueeMapper, conquestRewardMapper, earnableShardsMapper)
  {
  }

  protected override Character Create(InternalCharacter earnable,
    EarnableLocation[] earnableLocations,
    Marquee? marquee,
    ConquestReward? conquestReward,
    EarnableShards? earnableShards)
  {
    return new Character(earnable.Id,
      earnable.Name,
      earnableLocations,
      earnable.IsAccelerated,
      marquee,
      conquestReward,
      earnableShards);
  }

  protected override InternalCharacter Create(Character earnable,
    List<InternalEarnableLocation> earnableLocations)
  {
    var character = new InternalCharacter
    {
      Id = earnable.Id,
      Name = earnable.Name,
      Locations = earnableLocations,
      IsAccelerated = earnable.IsAccelerated,
    };

    return character;
  }
}
