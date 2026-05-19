using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresUserRepository : IUserRepository
{
  private readonly IPostgresDbContext _dbContext;
  private readonly IIdGenerator _idGenerator;

  public PostgresUserRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator)
  {
    _dbContext = dbContext;
    _idGenerator = idGenerator;
  }

  public async Task<IEnumerable<User>> GetUsers()
  {
    return await _dbContext.Users.ToListAsync();
  }

  public async Task<User> CreateUser(string email, string password)
  {
    // TODO: Hash password
    var user = new User(_idGenerator.CreateId(),
      email,
      password);

    await _dbContext.Users.AddAsync(user);
    await _dbContext.SaveChangesAsync();
    return user;
  }
}
