using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
using SwgohApi.Infrastructure.Postgres;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public abstract class AbstractPostgresRepositoryTests
{
  protected readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  protected readonly Mock<IPostgresDbContext> _mockDbContext;
  protected readonly Mock<IIdGenerator> _mockIdGenerator;

  protected AbstractPostgresRepositoryTests()
  {
    _mockDbContext = _mockRepository.Create<IPostgresDbContext>(MockBehavior.Loose);
    _mockIdGenerator = _mockRepository.Create<IIdGenerator>();
  }

  protected Mock<DbSet<T>> CreateMockDbSet<T>(Expression<Func<IPostgresDbContext, DbSet<T>>> expression)
  where T : class
  {
    var mockDbSet = _mockRepository.Create<DbSet<T>>(MockBehavior.Loose);
    _mockDbContext.Setup(expression)
      .Returns(mockDbSet.Object);

    return mockDbSet;
  }

  protected void SetupMockEntities<T>(Expression<Func<IPostgresDbContext, DbSet<T>>> expression,
    IEnumerable<T> entities)
    where T : class
  {
    _mockDbContext.Setup(expression)
      .ReturnsDbSet(entities);
  }
}
