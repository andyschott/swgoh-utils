using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresCharacterRepository : PostgresEarnableRepository<Character>, ICharacterRepository
{
  public PostgresCharacterRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator)
  : base(dbContext, idGenerator)
  {
  }

  protected override DbSet<Character> DbSet => _dbContext.Characters;

  public async Task<Character> CreateCharacter(string name,
    IEnumerable<EarnableLocation> locations,
    bool isAccelerated)
  {
    return await CreateEarnable(name,
      locations,
      character =>
      {
        character.IsAccelerated = isAccelerated;
      });
  }
}
