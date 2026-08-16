using System.Runtime;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;
using SwgohApi.TestUtilities;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresConquestRewardRepositoryTests : AbstractPostgresRepositoryTests
{
  private readonly PostgresConquestRewardRepository _repository;

  public  PostgresConquestRewardRepositoryTests()
  {
    _repository = new PostgresConquestRewardRepository(_mockDbContext.Object,
      _mockIdGenerator.Object);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateConquestReward_Successful(string id,
    Character character,
    ConquestRewardPhase rewardPhase,
    DateOnly initialUnlockDate,
    DateOnly finalRewardCrateDate,
    DateOnly provingGroundsDate)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.ConquestRewards);

    var result = await _repository.CreateConquestReward(character,
      rewardPhase,
      initialUnlockDate,
      finalRewardCrateDate,
      provingGroundsDate);

    Assert.Equal(id, result.Id);
    Assert.Equal(character.Id, result.CharacterId);
    Assert.Same(character, result.Character);
    Assert.Null(result.ShipId);
    Assert.Null(result.Ship);
    Assert.Equal(rewardPhase, result.RewardPhase);
    Assert.Equal(initialUnlockDate, result.InitialUnlockDate);
    Assert.Equal(finalRewardCrateDate, result.FinalRewardCreateDate);
    Assert.Equal(provingGroundsDate, result.ProvingGroundsDate);

    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(),
      Times.Once);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateConquestReward_Ship_Successful(string id,
    Ship ship,
    ConquestRewardPhase rewardPhase,
    DateOnly initialUnlockDate,
    DateOnly finalRewardCrateDate,
    DateOnly provingGroundsDate)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.ConquestRewards);

    var result = await _repository.CreateConquestReward(ship,
      rewardPhase,
      initialUnlockDate,
      finalRewardCrateDate,
      provingGroundsDate);

    Assert.Equal(id, result.Id);
    Assert.Null(result.CharacterId);
    Assert.Null(result.Character);
    Assert.Equal(ship.Id, result.ShipId);
    Assert.Same(ship, result.Ship);
    Assert.Equal(rewardPhase, result.RewardPhase);
    Assert.Equal(initialUnlockDate, result.InitialUnlockDate);
    Assert.Equal(finalRewardCrateDate, result.FinalRewardCreateDate);
    Assert.Equal(provingGroundsDate, result.ProvingGroundsDate);

    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(),
      Times.Once);
  }

  [Theory, SwgohApiAutoData]
  public async Task SaveMarquee_Successful(ConquestReward conquestReward)
  {
    var mockConuestRewardDbSet = CreateMockDbSet(dbContext => dbContext.ConquestRewards);

    var exception = await Record.ExceptionAsync(() => _repository.SaveConquestReward(conquestReward));

    Assert.Null(exception);
    mockConuestRewardDbSet.Verify(dbSet => dbSet.Update(conquestReward), Times.Once);
    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
  }
}
