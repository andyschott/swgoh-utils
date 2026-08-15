using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface IConquestRewardRepository
{
  Task<ConquestReward> CreateConquestReward(Earnable earnable,
    ConquestRewardPhase rewardPhase,
    DateOnly initialUnlockDate,
    DateOnly finalRewardCrateDate,
    DateOnly provingGroundsDate);
}
