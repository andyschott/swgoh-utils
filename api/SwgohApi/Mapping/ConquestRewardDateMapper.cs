using SwgohApi.Models.Earnables;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;
using InternalConquestRewardPhase = SwgohApi.Infrastructure.Models.ConquestRewardPhase;

namespace SwgohApi.Mapping;

public class ConquestRewardDateMapper : IMapper<InternalConquestReward, ConquestRewardDate>
{
  private readonly IMapper<InternalConquestRewardPhase, ConquestRewardPhase> _conquestRewardPhaseMapper;

  public ConquestRewardDateMapper(IMapper<InternalConquestRewardPhase, ConquestRewardPhase> conquestRewardPhaseMapper)
  {
    _conquestRewardPhaseMapper = conquestRewardPhaseMapper;
  }

  public ConquestRewardDate MapTo(InternalConquestReward source)
  {
    var name = source.Character?.Name ?? source.Ship?.Name;
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("ConquestReward must be associated with a Character or Ship name.", nameof(source));
    }

    return new ConquestRewardDate(name,
      _conquestRewardPhaseMapper.MapTo(source.RewardPhase),
      source.InitialUnlockDate,
      source.FinalRewardCreateDate,
      source.ProvingGroundsDate);
  }

  public InternalConquestReward MapFrom(ConquestRewardDate destination)
  {
    throw new NotImplementedException();
  }
}
