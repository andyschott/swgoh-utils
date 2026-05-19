using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresUserRepositoryTests
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IPostgresDbContext> _mockDbContext;
  private readonly Mock<IIdGenerator> _mockIdGenerator;

  private readonly PostgresUserRepository _userRepository;

  public PostgresUserRepositoryTests()
  {
    _mockDbContext = _mockRepository.Create<IPostgresDbContext>(MockBehavior.Loose);
    _mockIdGenerator = new Mock<IIdGenerator>();

    _userRepository = new PostgresUserRepository(_mockDbContext.Object,
      _mockIdGenerator.Object);
  }

  [Theory, AutoData]
  public async Task GetUsers_Successful(User[] users)
  {
    _mockDbContext.Setup(dbContext => dbContext.Users)
      .ReturnsDbSet(users);

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
    string password)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    var mockUsersDb = _mockRepository.Create<DbSet<User>>(MockBehavior.Loose);
    _mockDbContext.Setup(dbContext => dbContext.Users)
      .Returns(mockUsersDb.Object);

    var result = await _userRepository.CreateUser(email, password);

    Assert.Equal(id, result.Id);
    Assert.Equal(email, result.Email);
    Assert.Equal(password, result.Password);

    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
  }
}
