using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresShipRepositoryTests : AbstractPostgresRepositoryTests
{
  private readonly PostgresShipRepository _repository;

  public PostgresShipRepositoryTests()
  {
    _repository = new PostgresShipRepository(_mockDbContext.Object,
      _mockIdGenerator.Object);
  }

  [Theory, AutoData]
  public async Task CreateShip_Successful(string id,
    string name,
    EarnableLocation[] locations)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.Ships);

    var result = await _repository.CreateShip(name, locations);

    Assert.Equal(id, result.Id);
    Assert.Equal(name, result.Name);
    Assert.Equal(locations, result.Locations);
  }

  [Theory, AutoData]
  public async Task GetShipByName_Successful(Ship Ship)
  {
    SetupMockEntities(dbContext => dbContext.Ships, [Ship]);

    var result = await _repository.GetShipByName(Ship.Name);

    Assert.Same(Ship, result);
  }

  [Theory, AutoData]
  public async Task GetShipByName_NotFound_ReturnsNull(Ship Ship,
    string name)
  {
    SetupMockEntities(dbContext => dbContext.Ships, [Ship]);

    var result = await _repository.GetShipByName(name);

    Assert.Null(result);
  }
}
