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
      .Include(c => c.Marquee)
      .FirstOrDefaultAsync(c => c.Id == id);
  }
}
