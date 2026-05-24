using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;

namespace SwgohApi.Tests.Mappers;

public class FarmingStatusMapperTests
  : AbstractEnumMapperTests<InternalFarmingStatus, FarmingStatus, FarmingStatusMapper>
{
}
