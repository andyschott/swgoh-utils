using SwgohApi.Models.Earnables;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;

namespace SwgohApi.Mapping;

public class FarmingStatusMapper : IMapper<InternalFarmingStatus, FarmingStatus>
{
  public FarmingStatus MapTo(InternalFarmingStatus source)
  {
    return source switch
    {
      InternalFarmingStatus.Backlog => FarmingStatus.Backlog,
      InternalFarmingStatus.Active => FarmingStatus.Active,
      InternalFarmingStatus.Done => FarmingStatus.Done,
      _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
    };
  }

  public InternalFarmingStatus MapFrom(FarmingStatus destination)
  {
    return destination switch
    {
      FarmingStatus.Backlog => InternalFarmingStatus.Backlog,
      FarmingStatus.Active => InternalFarmingStatus.Active,
      FarmingStatus.Done => InternalFarmingStatus.Done,
      _ => throw new ArgumentOutOfRangeException(nameof(destination), destination, null)
    };
  }
}
