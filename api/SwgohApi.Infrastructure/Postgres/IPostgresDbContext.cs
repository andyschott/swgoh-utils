using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public interface IPostgresDbContext
{
  DbSet<User> Users { get; }

  Task<int> SaveChangesAsync();
}
