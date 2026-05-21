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
    return new InternalCharacter(earnable.Id,
      earnable.Name,
      earnableLocations,
      earnable.IsAccelerated);
  }
}
