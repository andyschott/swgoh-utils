using SwgohApi.Models.Earnables;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;

namespace SwgohApi.Mapping;

public class EarnableShardsMapper : IMapper<InternalEarnableShards, EarnableShards>
{
  private readonly IMapper<InternalFarmingStatus, FarmingStatus> _farmingStatusMapper;

  public EarnableShardsMapper(IMapper<InternalFarmingStatus, FarmingStatus> farmingStatusMapper)
  {
    _farmingStatusMapper = farmingStatusMapper;
  }

  public EarnableShards MapTo(InternalEarnableShards source)
  {
    return new EarnableShards(source.Id,
      source.Shards,
      _farmingStatusMapper.MapTo(source.FarmingStatus));
  }

  public InternalEarnableShards MapFrom(EarnableShards destination)
  {
    return new InternalEarnableShards
    {
      Id = destination.Id,
      Shards = destination.Shards,
      FarmingStatus = _farmingStatusMapper.MapFrom(destination.FarmingStatus)
    };
  }
}
