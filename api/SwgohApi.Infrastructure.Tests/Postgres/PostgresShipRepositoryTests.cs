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
  public async Task GetShipByName_Successful(Ship ship)
  {
    SetupMockEntities(dbContext => dbContext.Ships, [ship]);

    var result = await _repository.GetShipByName(ship.Name);

    Assert.Same(ship, result);
  }

  [Theory, AutoData]
  public async Task GetShipByName_NotFound_ReturnsNull(Ship ship,
    string name)
  {
    SetupMockEntities(dbContext => dbContext.Ships, [ship]);

    var result = await _repository.GetShipByName(name);

    Assert.Null(result);
  }

  [Theory, AutoData]
  public async Task GetShip_Successful(Ship ship)
  {
    SetupMockEntities(dbContext => dbContext.Ships, [ship]);

    var result = await _repository.GetShip(ship.Id);

    Assert.Same(ship, result);
  }

  [Theory, AutoData]
  public async Task GetShip_NotFound_ReturnsNull(Ship ship,
    string id)
  {
    SetupMockEntities(dbContext => dbContext.Ships, [ship]);

    var result = await _repository.GetShip(id);

    Assert.Null(result);
  }

  [Theory, AutoData]
  public async Task SaveShip_Successful(Ship ship)
  {
    SetupMockEntities(dbContext => dbContext.Ships, [ship]);

    var exception = await Record.ExceptionAsync(() => _repository.SaveShip(ship));

    Assert.Null(exception);
    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
  }
}
