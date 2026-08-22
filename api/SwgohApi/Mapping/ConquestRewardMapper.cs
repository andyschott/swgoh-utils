using SwgohApi.Models.Earnables;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;
using InternalConquestRewardPhase = SwgohApi.Infrastructure.Models.ConquestRewardPhase;

namespace SwgohApi.Mapping;

public class ConquestRewardMapper : IMapper<InternalConquestReward, ConquestReward>
{
  private readonly IMapper<InternalConquestRewardPhase, ConquestRewardPhase> _rewardPhaseMapper;

  public ConquestRewardMapper(IMapper<InternalConquestRewardPhase, ConquestRewardPhase> rewardPhaseMapper)
  {
    _rewardPhaseMapper = rewardPhaseMapper;
  }

  public ConquestReward MapTo(InternalConquestReward source)
  {
    return new ConquestReward(source.Id,
      _rewardPhaseMapper.MapTo(source.RewardPhase),
      source.InitialUnlockDate,
      source.FinalRewardCreateDate,
      source.ProvingGroundsDate);
  }

  public InternalConquestReward MapFrom(ConquestReward destination)
  {
    return new InternalConquestReward
    {
      Id = destination.Id,
      RewardPhase = _rewardPhaseMapper.MapFrom(destination.RewardPhase),
      InitialUnlockDate = destination.InitialUnlockDate,
      FinalRewardCreateDate = destination.FinalRewardCreateDate,
      ProvingGroundsDate = destination.ProvingGroundsDate,
    };
  }
}
