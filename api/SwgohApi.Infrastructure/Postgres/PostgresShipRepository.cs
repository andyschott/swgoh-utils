using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresShipRepository : PostgresEarnableRepository<Ship>, IShipRepository
{
  public PostgresShipRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator)
  : base(dbContext, idGenerator)
  {
  }

  protected override DbSet<Ship> DbSet => _dbContext.Ships;

  public async Task<Ship> CreateShip(string name,
    IEnumerable<EarnableLocation> locations)
  {
    return await CreateEarnable(name, locations);
  }

  public Task<Ship?> GetShip(string id)
  {
    return _dbContext.Ships
      .Include(s => s.Marquee)
      .FirstOrDefaultAsync(c => c.Id == id);
  }

  public async Task SaveShip(Ship ship)
  {
    _dbContext.Ships.Update(ship);
    await _dbContext.SaveChangesAsync();
  }
}
