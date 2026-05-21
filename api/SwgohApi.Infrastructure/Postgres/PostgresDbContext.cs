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

  Task<int> IPostgresDbContext.SaveChangesAsync() => base.SaveChangesAsync();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
      .HasIndex(user => user.Email)
      .IsUnique();

    modelBuilder.Entity<RefreshToken>()
      .HasIndex(token => token.TokenHash)
      .IsUnique();

    modelBuilder.Entity<RefreshToken>()
      .HasIndex(token => new { token.UserId, token.RevokedAtUtc, token.ExpiresAtUtc });

    modelBuilder.Entity<Character>()
      .HasIndex(character => character.Name)
      .IsUnique();

    modelBuilder.Entity<Ship>()
      .HasIndex(ship => ship.Name)
      .IsUnique();
  }
}
