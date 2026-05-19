using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface IUserRepository
{
  Task<IEnumerable<User>> GetUsers();
  Task<User> CreateUser(string email, string password);
}
