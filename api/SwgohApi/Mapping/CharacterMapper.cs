using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Mapping;

public class CharacterMapper : EarnableMapper<InternalCharacter, Character>
{
  public CharacterMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper,
    IMapper<InternalMarquee, Marquee> marqueeMapper,
    IMapper<InternalEarnableShards, EarnableShards> earnableShardsMapper)
  : base(locationMapper, marqueeMapper,earnableShardsMapper)
  {
  }

  protected override Character Create(InternalCharacter earnable,
    EarnableLocation[] earnableLocations,
    Marquee? marquee,
    EarnableShards? earnableShards)
  {
    return new Character(earnable.Id,
      earnable.Name,
      earnableLocations,
      earnable.IsAccelerated,
      marquee,
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
