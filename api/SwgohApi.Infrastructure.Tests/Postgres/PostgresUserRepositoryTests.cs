using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresUserRepositoryTests : AbstractPostgresRepositoryTests
{
  private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;

  private readonly PostgresUserRepository _userRepository;

  public PostgresUserRepositoryTests()
  {
    _mockPasswordHasher = _mockRepository.Create<IPasswordHasher<User>>();

    _userRepository = new PostgresUserRepository(_mockDbContext.Object,
      _mockIdGenerator.Object,
      _mockPasswordHasher.Object);
  }

  [Theory, AutoData]
  public async Task GetUsers_Successful(User[] users)
  {
    SetupMockEntities(dbContext => dbContext.Users, users);

    var result = (await _userRepository.GetUsers())
      .ToArray();

    Assert.Equal(users.Length, result.Length);
    Assert.All(users, user =>
    {
      Assert.Contains(user, result);
    });
  }

  [Theory, AutoData]
  public async Task CreateUser_Successful(string id,
    string email,
    string password,
    string hashedPassword)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);
    _mockPasswordHasher.Setup(hasher => hasher.HashPassword(
      It.Is<User>(user => VerifyUser(user, id, email, string.Empty)),
      password))
      .Returns(hashedPassword);

    CreateMockDbSet(dbContext => dbContext.Users);

    var result = await _userRepository.CreateUser(email, password);

    Assert.Equal(id, result.Id);
    Assert.Equal(email, result.Email);
    Assert.Equal(hashedPassword, result.Password);

    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
  }

  [Theory, AutoData]
  public async Task GetUserByEmail_Successful(User user)
  {
    SetupMockEntities(dbContext => dbContext.Users, [user]);

    var result = await _userRepository.GetUserByEmail(user.Email);

    Assert.Same(user, result);
  }

  [Theory, AutoData]
  public async Task GetUserByEmail_NotFound_ReturnsNull(User user,
    string email)
  {
    SetupMockEntities(dbContext => dbContext.Users, [user]);

    var result = await _userRepository.GetUserByEmail(email);

    Assert.Null(result);
  }

  [Theory, AutoData]
  public async Task GetUserById_Successful(User user)
  {
    SetupMockEntities(dbContext => dbContext.Users, [user]);

    var result = await _userRepository.GetUserById(user.Id);

    Assert.Same(user, result);
  }

  [Theory, AutoData]
  public async Task GetUserById_NotFound_ReturnsNull(User user,
    string id)
  {
    SetupMockEntities(dbContext => dbContext.Users, [user]);

    var result = await _userRepository.GetUserById(id);

    Assert.Null(result);
  }

  [Theory, AutoData]
  public async Task SaveUser_Successful(User user)
  {
    SetupMockEntities(dbContext => dbContext.Users, [user]);

    var exception = await Record.ExceptionAsync(() => _userRepository.SaveUser(user));

    Assert.Null(exception);
    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
  }

  [Theory, AutoData]
  public async Task DeleteUser_Successful(User user)
  {
    SetupMockEntities(dbContext => dbContext.Users, [user]);

    var result = await _userRepository.DeleteUser(user.Id);

    Assert.True(result);
  }

  [Theory, AutoData]
  public async Task DeleteUser_NotFound_ReturnsFalse(User user, string id)
  {
    SetupMockEntities(dbContext => dbContext.Users, [user]);

    var result = await _userRepository.DeleteUser(id);

    Assert.False(result);
  }

  private static bool VerifyUser(User user,
    string id,
    string email,
    string password)
  {
    return user.Id == id &&
           user.Email == email &&
           user.Password == password;
  }
}
