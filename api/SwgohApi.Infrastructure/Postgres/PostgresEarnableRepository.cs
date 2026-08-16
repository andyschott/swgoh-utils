using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public abstract class PostgresEarnableRepository<T> : IEarnableRepository<T>
where T : Earnable, new()
{
  protected readonly IPostgresDbContext _dbContext;
  protected readonly IIdGenerator _idGenerator;

  protected PostgresEarnableRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator)
  {
    _dbContext = dbContext;
    _idGenerator = idGenerator;
  }

  protected abstract DbSet<T> DbSet { get; }

  public async Task<IEnumerable<T>> GetEarnables()
  {
    return await DbSet.ToListAsync();
  }

  public async Task<T?> GetEarnable(string id)
  {
    return await DbSet
      .Include(e => e.Marquee)
      .Include(e => e.ConquestReward)
      .FirstOrDefaultAsync(e => e.Id == id);
  }

  public async Task<T?> GetEarnableByName(string name)
  {
    return await DbSet
      .Include(e => e.Marquee)
      .FirstOrDefaultAsync(e => e.Name == name);
  }

  public async Task SaveEarnable(T earnable)
  {
    DbSet.Update(earnable);
    await _dbContext.SaveChangesAsync();
  }

  public async Task<IEnumerable<T>> GetEarnablesForUser(User user)
  {
    return await DbSet
      .Include(e => e.EarnableShards.Where(es => es.UserId == user.Id))
      .ToListAsync();
  }

  public async Task<T?> GetEarnableForUser(string id, User user)
  {
    return await DbSet
      .Include(e => e.EarnableShards.Where(es => es.UserId == user.Id))
      .FirstOrDefaultAsync(e => e.Id == id);
  }

  protected async Task<T> CreateEarnable(string name,
    IEnumerable<EarnableLocation> locations,
    Action<T>? updateEarnable = null)
  {
    var earnable = new T
    {
      Id = _idGenerator.CreateId(),
      Name = name,
      Locations = locations.ToList(),
      Marquee = null
    };
    updateEarnable?.Invoke(earnable);

    await DbSet.AddAsync(earnable);
    await _dbContext.SaveChangesAsync();

    return earnable;
  }
}
