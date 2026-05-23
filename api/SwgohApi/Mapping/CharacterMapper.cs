using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Mapping;

public class CharacterMapper : EarnableMapper<InternalCharacter, Character>
{
  public CharacterMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper)
  : base(locationMapper)
  {
  }

  protected override Character Create(InternalCharacter earnable,
    EarnableLocation[] earnableLocations)
  {
    return new Character(earnable.Id,
      earnable.Name,
      earnableLocations,
      earnable.IsAccelerated);
  }

  protected override InternalCharacter Create(Character earnable,
    List<InternalEarnableLocation> earnableLocations)
  {
    // TODO: Map marquee eventually
    return new InternalCharacter
    {
      Id = earnable.Id,
      Name = earnable.Name,
      Locations = earnableLocations,
      IsAccelerated = earnable.IsAccelerated,
    };
  }
}
