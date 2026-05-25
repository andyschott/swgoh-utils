using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface ICharacterRepository : IEarnableRepository<Character>
{
  Task<Character> CreateCharacter(string name,
    IEnumerable<EarnableLocation> locations,
    bool isAccelerated);
  Task<Character?> GetCharacter(string id);
  Task SaveCharacter(Character character);
  Task<IEnumerable<Character>> GetCharactersForUser(User user);
  Task<Character?> GetCharacterForUser(string id, User user);
}
