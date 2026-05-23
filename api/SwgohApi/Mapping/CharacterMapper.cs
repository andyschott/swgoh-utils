using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Mapping;

public class CharacterMapper : EarnableMapper<InternalCharacter, Character>
{
  public CharacterMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper,
    IMapper<InternalMarquee, Marquee> marqueeMapper)
  : base(locationMapper,  marqueeMapper)
  {
  }

  protected override Character Create(InternalCharacter earnable,
    EarnableLocation[] earnableLocations,
    Marquee? marquee)
  {
    return new Character(earnable.Id,
      earnable.Name,
      earnableLocations,
      earnable.IsAccelerated,
      marquee);
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
