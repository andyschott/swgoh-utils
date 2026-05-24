using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;

namespace SwgohApi.Tests.Mappers;

public sealed class EarnableShardsMapperTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IMapper<InternalFarmingStatus, FarmingStatus>> _mockFarmingStatusMapper;

  private readonly EarnableShardsMapper _mapper;

  public EarnableShardsMapperTests()
  {
    _mockFarmingStatusMapper = _mockRepository.Create<IMapper<InternalFarmingStatus, FarmingStatus>>();

    _mapper = new EarnableShardsMapper(_mockFarmingStatusMapper.Object);
  }

  [Theory, SwgohApiAutoData]
  public void MapTo_Successful(InternalEarnableShards internalEarnableShards,
    FarmingStatus farmingStatus)
  {
    _mockFarmingStatusMapper.Setup(mapper => mapper.MapTo(internalEarnableShards.FarmingStatus))
      .Returns(farmingStatus);

    var result = _mapper.MapTo(internalEarnableShards);

    Assert.Equal(internalEarnableShards.Id, result.Id);
    Assert.Equal(internalEarnableShards.Shards, result.Shards);
    Assert.Equal(farmingStatus, result.FarmingStatus);
  }

  [Theory, SwgohApiAutoData]
  public void MapFrom_Successful(EarnableShards earnableShards,
    InternalFarmingStatus farmingStatus)
  {
    _mockFarmingStatusMapper.Setup(mapper => mapper.MapFrom(earnableShards.FarmingStatus))
      .Returns(farmingStatus);

    var result = _mapper.MapFrom(earnableShards);

    Assert.Equal(earnableShards.Id, result.Id);
    Assert.Equal(earnableShards.Shards, result.Shards);
    Assert.Equal(farmingStatus, result.FarmingStatus);
  }

  public void Dispose() => _mockRepository.VerifyAll();
}
