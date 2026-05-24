using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresUserRepository : IUserRepository
{
  private readonly IPostgresDbContext _dbContext;
  private readonly IIdGenerator _idGenerator;
  private readonly IPasswordHasher<User> _passwordHasher;

  public PostgresUserRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator,
    IPasswordHasher<User> passwordHasher)
  {
    _dbContext = dbContext;
    _idGenerator = idGenerator;
    _passwordHasher = passwordHasher;
  }

  public async Task<IEnumerable<User>> GetUsers()
  {
    return await _dbContext.Users.ToListAsync();
  }

  public async Task<User> CreateUser(string email, string password)
  {
    var user = new User
    {
      Id = _idGenerator.CreateId(),
      Email = email,
      IsAdmin = false,
    };

    user.Password = _passwordHasher.HashPassword(user, password);

    await _dbContext.Users.AddAsync(user);
    await _dbContext.SaveChangesAsync();
    return user;
  }

  public async Task<User?> GetUserByEmail(string email)
  {
    return await _dbContext.Users
      .FirstOrDefaultAsync(u => u.Email == email);
  }

  public async Task<User?> GetUserById(string id)
  {
    return await _dbContext.Users
      .FirstOrDefaultAsync(x => x.Id == id);
  }

  public async Task SaveUser(User user)
  {
    _dbContext.Users.Update(user);
    await _dbContext.SaveChangesAsync();
  }

  public async Task<bool> DeleteUser(string id)
  {
    var user = await GetUserById(id);
    if (user is null)
    {
      return false;
    }

    _dbContext.Users.Remove(user);
    await _dbContext.SaveChangesAsync();

    return true;
  }
}
