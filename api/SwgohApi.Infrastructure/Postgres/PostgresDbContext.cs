using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresDbContext : DbContext, IPostgresDbContext
{
  public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
    : base(options)
  {
  }

  public DbSet<User> Users { get; set; }
  public DbSet<RefreshToken> RefreshTokens { get; set; }
  public DbSet<Character> Characters { get; set; }
  public DbSet<Ship> Ships { get; set; }
  public DbSet<Marquee> Marquees { get; set; }
  public DbSet<EarnableShards> EarnableShards { get; set; }

  Task<int> IPostgresDbContext.SaveChangesAsync() => base.SaveChangesAsync();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgresDbContext).Assembly);
  }
}
