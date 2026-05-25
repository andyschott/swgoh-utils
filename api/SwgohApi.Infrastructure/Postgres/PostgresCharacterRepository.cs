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

  public async Task<IEnumerable<Character>> GetCharactersForUser(User user)
  {
    return await _dbContext.Characters
      .Include(c => c.EarnableShards)
      .Where(c => c.EarnableShards == null || c.EarnableShards.UserId == user.Id)
      .ToListAsync();
  }

  public async Task<Character?> GetCharacterForUser(string id, User user)
  {
    return await _dbContext.Characters
      .Include(c => c.EarnableShards)
      .FirstOrDefaultAsync(c => c.Id == id);
  }
}
