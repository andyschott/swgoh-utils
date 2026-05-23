using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresMarqueeRepository : IMarqueeRepository
{
  private readonly IPostgresDbContext _dbContext;
  private readonly IIdGenerator _idGenerator;

  public PostgresMarqueeRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator)
  {
    _dbContext = dbContext;
    _idGenerator = idGenerator;
  }
  public async Task<Marquee> CreateMarquee(Earnable earnable,
    DateOnly introductionDate,
    DateOnly marqueeEventDate,
    DateOnly shipmentDate,
    DateOnly farmDate,
    DateOnly? accelerationDate)
  {
    var marquee = new Marquee
    {
      Id = _idGenerator.CreateId(),
      IntroductionDate = introductionDate,
      MarqueeEventDate = marqueeEventDate,
      ShipmentDate = shipmentDate,
      FarmDate = farmDate,
      AccelerationDate = accelerationDate
    };

    if (earnable is Character character)
    {
      marquee.CharacterId = character.Id;
      marquee.Character = character;

      marquee.ShipId = null;
      marquee.Ship = null;
    }
    else if (earnable is Ship ship)
    {
      marquee.ShipId = ship.Id;
      marquee.Ship = ship;

      marquee.CharacterId = null;
      marquee.Character = null;
    }

    await _dbContext.Marquees.AddAsync(marquee);
    await _dbContext.SaveChangesAsync();
    return marquee;
  }

  public async Task<Marquee?> GetMarquee(string id)
  {
    return await _dbContext.Marquees
      .FirstOrDefaultAsync(m => m.Id == id);
  }

  public async Task SaveMarquee(Marquee marquee)
  {
    _dbContext.Marquees.Update(marquee);
    await _dbContext.SaveChangesAsync();
  }
}
