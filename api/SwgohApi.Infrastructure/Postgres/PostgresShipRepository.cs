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

  public async Task<Ship> CreateShip(string name,
    IEnumerable<EarnableLocation> locations,
    Marquee? marquee)
  {
    var ship = new Ship
    {
      Id = _idGenerator.CreateId(),
      Name = name,
      Locations = locations.ToList(),
      Marquee = marquee
    };

    await _dbContext.Ships.AddAsync(ship);
    await _dbContext.SaveChangesAsync();
    return ship;
  }

  public async Task<Ship?> GetShipByName(string name)
  {
    return await _dbContext.Ships
      .Include(s => s.Marquee)
      .FirstOrDefaultAsync(c => c.Name == name);
  }

  public async Task<IEnumerable<Ship>> GetShips()
  {
    return await _dbContext.Ships.ToListAsync();
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
