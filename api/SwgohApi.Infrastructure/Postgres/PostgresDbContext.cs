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

  Task<int> IPostgresDbContext.SaveChangesAsync() => base.SaveChangesAsync();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
      .HasIndex(user => user.Email)
      .IsUnique();
  }
}
