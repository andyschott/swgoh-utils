using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;
using SwgohApi.TestUtilities;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresEarnableRepositoryTests : AbstractPostgresRepositoryTests
{
  private readonly TestPostgresEarnableRepository _repository;

  public PostgresEarnableRepositoryTests()
  {
    _repository = new TestPostgresEarnableRepository(_mockDbContext.Object,
      _mockIdGenerator.Object);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetEarnables_Successful(Character[] characters)
  {
    SetupMockEntities(dbContext => dbContext.Characters, characters);

    var result = await _repository.GetEarnables();

    Assert.Equal(characters, result);
  }

  class TestPostgresEarnableRepository : PostgresEarnableRepository<Character>
  {
    public TestPostgresEarnableRepository(IPostgresDbContext dbContext, IIdGenerator idGenerator)
      : base(dbContext, idGenerator)
    {
    }

    protected override DbSet<Character> DbSet => _dbContext.Characters;
  }
}
