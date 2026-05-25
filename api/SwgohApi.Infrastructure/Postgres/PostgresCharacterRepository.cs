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
    var character = new Character
    {
      Id = _idGenerator.CreateId(),
      Name = name,
      Locations = locations.ToList(),
      IsAccelerated = isAccelerated,
      Marquee = null
    };

    await _dbContext.Characters.AddAsync(character);
    await _dbContext.SaveChangesAsync();
    return character;
  }

  public async Task<Character?> GetCharacterByName(string name)
  {
    return await _dbContext.Characters
      .Include(c => c.Marquee)
      .FirstOrDefaultAsync(c => c.Name == name);
  }

  public async Task<IEnumerable<Character>> GetCharacters()
  {
    return await _dbContext.Characters.ToListAsync();
  }

  public async Task<Character?> GetCharacter(string id)
  {
    return await _dbContext.Characters
      .Include(c => c.Marquee)
      .FirstOrDefaultAsync(c => c.Id == id);
  }

  public async Task SaveCharacter(Character character)
  {
    _dbContext.Characters.Update(character);
    await _dbContext.SaveChangesAsync();
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
