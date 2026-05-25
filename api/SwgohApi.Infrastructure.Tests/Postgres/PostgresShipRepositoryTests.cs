using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;
using SwgohApi.TestUtilities;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresShipRepositoryTests : AbstractPostgresRepositoryTests
{
  private readonly PostgresShipRepository _repository;

  public PostgresShipRepositoryTests()
  {
    _repository = new PostgresShipRepository(_mockDbContext.Object,
      _mockIdGenerator.Object);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateShip_Successful(string id,
    string name,
    EarnableLocation[] locations)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.Ships);

    var result = await _repository.CreateShip(name,
      locations);

    Assert.Equal(id, result.Id);
    Assert.Equal(name, result.Name);
    Assert.Equal(locations, result.Locations);
  }
}
