using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface IEarnableShardsRepository
{
  Task<EarnableShards> CreateEarnableShards(User user,
    Earnable earnable,
    int shards,
    FarmingStatus farmingStatus);
  Task<EarnableShards?> GetEarnableShards(string id);
  Task SaveEarnableShards(EarnableShards earnableShards);
}
