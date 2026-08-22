using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresConquestRewardRepository : IConquestRewardRepository
{
  private readonly IPostgresDbContext _dbContext;
  private readonly IIdGenerator _idGenerator;

  public PostgresConquestRewardRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator)
  {
    _dbContext = dbContext;
    _idGenerator = idGenerator;
  }

  public async Task<ConquestReward> CreateConquestReward(Earnable earnable,
    ConquestRewardPhase rewardPhase,
    DateOnly initialUnlockDate,
    DateOnly finalRewardCrateDate,
    DateOnly provingGroundsDate)
  {
    var conquestReward = new ConquestReward
    {
      Id = _idGenerator.CreateId(),
      RewardPhase = rewardPhase,
      InitialUnlockDate = initialUnlockDate,
      FinalRewardCreateDate = finalRewardCrateDate,
      ProvingGroundsDate = provingGroundsDate
    };

    if (earnable is Character character)
    {
      conquestReward.CharacterId = character.Id;
      conquestReward.Character = character;

      conquestReward.ShipId = null;
      conquestReward.ShipId = null;
    }
    else if (earnable is Ship ship)
    {
      conquestReward.ShipId = ship.Id;
      conquestReward.Ship = ship;

      conquestReward.CharacterId = null;
      conquestReward.Character = null;
    }

    await _dbContext.ConquestRewards.AddAsync(conquestReward);
    await _dbContext.SaveChangesAsync();
    return conquestReward;
  }

  public async Task<IEnumerable<ConquestReward>> GetConquestRewards()
  {
    return await _dbContext.ConquestRewards
      .Include(cr => cr.Character)
      .Include(cr => cr.Ship)
      .ToListAsync();
  }

  public async Task SaveConquestReward(ConquestReward conquestReward)
  {
    _dbContext.ConquestRewards.Update(conquestReward);
    await _dbContext.SaveChangesAsync();
  }
}
