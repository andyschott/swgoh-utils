using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface ICharacterRepository : IEarnableRepository<Character>
{
  Task<Character> CreateCharacter(string name,
    IEnumerable<EarnableLocation> locations,
    bool isAccelerated);
}
