using SwgohApi.Models.Earnables;
using InternalConquestRewardPhase = SwgohApi.Infrastructure.Models.ConquestRewardPhase;

namespace SwgohApi.Mapping;

public class ConquestRewardPhaseMapper : IMapper<InternalConquestRewardPhase, ConquestRewardPhase>
{
  public ConquestRewardPhase MapTo(InternalConquestRewardPhase source)
  {
    return source switch
    {
      InternalConquestRewardPhase.MainReward => ConquestRewardPhase.MainReward,
      InternalConquestRewardPhase.SecondaryReward => ConquestRewardPhase.SecondaryReward,
      InternalConquestRewardPhase.ConquestShipments => ConquestRewardPhase.ConquestShipments,
      InternalConquestRewardPhase.ProvingGrounds => ConquestRewardPhase.ProvingGrounds,
      _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
    };
  }

  public InternalConquestRewardPhase MapFrom(ConquestRewardPhase destination)
  {
    return destination switch
    {
      ConquestRewardPhase.MainReward => InternalConquestRewardPhase.MainReward,
      ConquestRewardPhase.SecondaryReward => InternalConquestRewardPhase.SecondaryReward,
      ConquestRewardPhase.ConquestShipments => InternalConquestRewardPhase.ConquestShipments,
      ConquestRewardPhase.ProvingGrounds => InternalConquestRewardPhase.ProvingGrounds,
      _ => throw new ArgumentOutOfRangeException(nameof(destination), destination, null)
    };
  }
}
