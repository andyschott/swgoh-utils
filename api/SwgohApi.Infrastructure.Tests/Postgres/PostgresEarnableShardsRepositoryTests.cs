using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;
using SwgohApi.TestUtilities;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresEarnableShardsRepositoryTests : AbstractPostgresRepositoryTests
{
  private readonly PostgresEarnableShardsRepository _repository;

  public PostgresEarnableShardsRepositoryTests()
  {
    _repository = new PostgresEarnableShardsRepository(_mockDbContext.Object,
      _mockIdGenerator.Object);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateEarnableShards_Successful(string id,
    User user,
    Character character,
    int shards,
    FarmingStatus farmingStatus)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.EarnableShards);

    var result = await _repository.CreateEarnableShards(user,
      character,
      shards,
      farmingStatus);

    Assert.Equal(id, result.Id);
    Assert.Equal(character.Id, result.CharacterId);
    Assert.Same(character, result.Character);
    Assert.Null(result.ShipId);
    Assert.Null(result.Ship);
    Assert.Equal(shards, result.Shards);
    Assert.Equal(farmingStatus, result.FarmingStatus);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateEarnableShards_Ship_Successful(string id,
    User user,
    Ship ship,
    int shards,
    FarmingStatus farmingStatus)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.EarnableShards);

    var result = await _repository.CreateEarnableShards(user,
      ship,
      shards,
      farmingStatus);

    Assert.Equal(id, result.Id);
    Assert.Null(result.CharacterId);
    Assert.Null(result.Character);
    Assert.Equal(ship.Id, result.ShipId);
    Assert.Same(ship, result.Ship);
    Assert.Equal(shards, result.Shards);
    Assert.Equal(farmingStatus, result.FarmingStatus);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetEarnableShards_Successful(EarnableShards earnableShards)
  {
    SetupMockEntities(dbContext => dbContext.EarnableShards, [earnableShards]);

    var result = await _repository.GetEarnableShards(earnableShards.Id);

    Assert.Same(earnableShards, result);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetEarnableShards_NotFound_ReturnsNull(EarnableShards earnableShards,
    string id)
  {
    SetupMockEntities(dbContext => dbContext.EarnableShards, [earnableShards]);

    var result = await _repository.GetEarnableShards(id);

    Assert.Null(result);
  }

  [Theory, SwgohApiAutoData]
  public async Task SaveEarnableShards_Successful(EarnableShards earnableShards)
  {
    var mockEarnableShardsSet = CreateMockDbSet(dbContext => dbContext.EarnableShards);

    var exception = await Record.ExceptionAsync(() => _repository.SaveEarnableShards(earnableShards));

    Assert.Null(exception);
    mockEarnableShardsSet.Verify(dbSet => dbSet.Update(earnableShards), Times.Once);
    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
  }
}
