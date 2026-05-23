using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresShipRepository : IShipRepository
{
  private readonly IPostgresDbContext _dbContext;
  private readonly IIdGenerator _idGenerator;

  public PostgresShipRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator)
  {
    _dbContext = dbContext;
    _idGenerator = idGenerator;
  }

  public async Task<Ship> CreateShip(string name, IEnumerable<EarnableLocation> locations)
  {
    var Ship = new Ship(_idGenerator.CreateId(),
      name,
      locations.ToList());

    await _dbContext.Ships.AddAsync(Ship);
    await _dbContext.SaveChangesAsync();
    return Ship;
  }

  public async Task<Ship?> GetShipByName(string name)
  {
    return await _dbContext.Ships
      .FirstOrDefaultAsync(c => c.Name == name);
  }

  public async Task<IEnumerable<Ship>> GetShips()
  {
    return await _dbContext.Ships.ToListAsync();
  }

  public Task<Ship?> GetShip(string id)
  {
    return _dbContext.Ships
      .FirstOrDefaultAsync(c => c.Id == id);
  }
}
