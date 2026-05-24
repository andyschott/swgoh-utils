using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface IEarnableShardsRepository
{
  Task<EarnableShards> CreateEarnableShards(User user,
    Earnable earnable,
    int shards,
    FarmingStatus farmingStatus);
}
