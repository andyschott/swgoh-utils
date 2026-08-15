using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;
using InternalConquestRewardPhase = SwgohApi.Infrastructure.Models.ConquestRewardPhase;

namespace SwgohApi.Tests.Mappers;

public sealed class ConquestRewardMapperTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IMapper<InternalConquestRewardPhase, ConquestRewardPhase>> _mockRewardPhaseMapper;

  private readonly ConquestRewardMapper _mapper;

  public ConquestRewardMapperTests()
  {
    _mockRewardPhaseMapper = _mockRepository.Create<IMapper<InternalConquestRewardPhase, ConquestRewardPhase>>();

    _mapper = new ConquestRewardMapper(_mockRewardPhaseMapper.Object);
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, SwgohApiAutoData]
  public void MapTo_Successful(InternalConquestReward source,
    ConquestRewardPhase rewardPhase)
  {
    _mockRewardPhaseMapper.Setup(mapper => mapper.MapTo(source.RewardPhase))
      .Returns(rewardPhase);

    var result = _mapper.MapTo(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(rewardPhase, result.RewardPhase);
    Assert.Equal(source.InitialUnlockDate, result.InitialUnlockDate);
    Assert.Equal(source.FinalRewardCreateDate, result.FinalRewardCreateDate);
    Assert.Equal(source.ProvingGroundsDate, result.ProvingGroundsDate);
  }

  [Theory, SwgohApiAutoData]
  public void MapFrom_Successful(ConquestReward source,
    InternalConquestRewardPhase rewardPhase)
  {
    _mockRewardPhaseMapper.Setup(mapper => mapper.MapFrom(source.RewardPhase))
      .Returns(rewardPhase);

    var result = _mapper.MapFrom(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(rewardPhase, result.RewardPhase);
    Assert.Equal(source.InitialUnlockDate, result.InitialUnlockDate);
    Assert.Equal(source.FinalRewardCreateDate, result.FinalRewardCreateDate);
    Assert.Equal(source.ProvingGroundsDate, result.ProvingGroundsDate);
  }
}
