using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Tests.Mappers;

public class EarnableLocationMapperTests
: AbstractEnumMapperTests<InternalEarnableLocation, EarnableLocation, EarnableLocationMapper>
{
}
