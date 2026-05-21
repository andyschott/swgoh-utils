using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public interface IPostgresDbContext
{
  DbSet<User> Users { get; }
  DbSet<RefreshToken> RefreshTokens { get; }
  DbSet<Character> Characters { get; }
  DbSet<Ship> Ships { get; }

  Task<int> SaveChangesAsync();
}
