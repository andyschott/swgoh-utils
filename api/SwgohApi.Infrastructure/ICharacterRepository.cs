using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface ICharacterRepository
{
  Task<Character> CreateCharacter(string name,
    IEnumerable<EarnableLocation> locations,
    bool isAccelerated);
  Task<Character?> GetCharacterByName(string name);
  Task<IEnumerable<Character>> GetCharacters();
  Task<Character?> GetCharacter(string id);
  Task SaveCharacter(Character character);
}
