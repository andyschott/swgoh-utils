using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface IUserRepository
{
  Task<IEnumerable<User>> GetUsers();
  Task<User> CreateUser(string email, string password);
  Task<User?> GetUserByEmail(string email);
  Task<User?> GetUserById(string id);
  Task SaveUser(User user);
  Task<bool> DeleteUser(string id);
}
