using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresCharacterRepository : ICharacterRepository
{
  private readonly IPostgresDbContext _dbContext;
  private readonly IIdGenerator _idGenerator;

  public PostgresCharacterRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator)
  {
    _dbContext = dbContext;
    _idGenerator = idGenerator;
  }

  public async Task<Character> CreateCharacter(string name,
    IEnumerable<EarnableLocation> locations,
    bool isAccelerated,
    Marquee? marquee)
  {
    var character = new Character(_idGenerator.CreateId(),
      name,
      locations.ToList(),
      isAccelerated,
      marquee);

    await _dbContext.Characters.AddAsync(character);
    await _dbContext.SaveChangesAsync();
    return character;
  }

  public async Task<Character?> GetCharacterByName(string name)
  {
    return await _dbContext.Characters
      .FirstOrDefaultAsync(c => c.Name == name);
  }

  public async Task<IEnumerable<Character>> GetCharacters()
  {
    return await _dbContext.Characters.ToListAsync();
  }

  public async Task<Character?> GetCharacter(string id)
  {
    return await _dbContext.Characters
      .FirstOrDefaultAsync(c => c.Id == id);
  }

  public async Task SaveCharacter(Character character)
  {
    _dbContext.Characters.Update(character);
    await _dbContext.SaveChangesAsync();
  }
}
