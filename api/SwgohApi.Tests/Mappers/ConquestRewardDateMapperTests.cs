using AutoFixture;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;
using InternalConquestRewardPhase = SwgohApi.Infrastructure.Models.ConquestRewardPhase;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Tests.Mappers;

public sealed class ConquestRewardDateMapperTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IMapper<InternalConquestRewardPhase, ConquestRewardPhase>> _mockRewardPhaseMapper;

  private readonly ConquestRewardDateMapper _mapper;

  public ConquestRewardDateMapperTests()
  {
    _mockRewardPhaseMapper = _mockRepository.Create<IMapper<InternalConquestRewardPhase, ConquestRewardPhase>>();

    _mapper = new ConquestRewardDateMapper(_mockRewardPhaseMapper.Object);
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, SwgohApiAutoData]
  public void MapTo_Character_Successful(InternalCharacter character,
    ConquestRewardPhase conquestRewardPhase,
    IFixture fixture)
  {
    var source = fixture.Build<InternalConquestReward>()
      .With(m => m.Character, character)
      .With(m => m.CharacterId, character.Id)
      .With(m => m.Ship, (InternalShip?)null)
      .With(m => m.ShipId, (string?)null)
      .Create();

    _mockRewardPhaseMapper.Setup(mapper => mapper.MapTo(source.RewardPhase))
      .Returns(conquestRewardPhase);

    var result = _mapper.MapTo(source);

    Assert.Equal(character.Name, result.Name);
    Assert.Equal(conquestRewardPhase, result.RewardPhase);
    Assert.Equal(source.InitialUnlockDate, result.InitialUnlockDate);
    Assert.Equal(source.FinalRewardCreateDate, result.FinalRewardCreateDate);
    Assert.Equal(source.ProvingGroundsDate, result.ProvingGroundsDate);
  }

  [Theory, SwgohApiAutoData]
  public void MapTo_Ship_Successful(InternalShip ship,
    ConquestRewardPhase conquestRewardPhase,
    IFixture fixture)
  {
    var source = fixture.Build<InternalConquestReward>()
      .With(m => m.Character, (InternalCharacter?)null)
      .With(m => m.CharacterId, (string?)null)
      .With(m => m.Ship, ship)
      .With(m => m.ShipId, ship.Id)
      .Create();

    _mockRewardPhaseMapper.Setup(mapper => mapper.MapTo(source.RewardPhase))
      .Returns(conquestRewardPhase);

    var result = _mapper.MapTo(source);

    Assert.Equal(ship.Name, result.Name);
    Assert.Equal(conquestRewardPhase, result.RewardPhase);
    Assert.Equal(source.InitialUnlockDate, result.InitialUnlockDate);
    Assert.Equal(source.FinalRewardCreateDate, result.FinalRewardCreateDate);
    Assert.Equal(source.ProvingGroundsDate, result.ProvingGroundsDate);
  }

  [Theory, SwgohApiAutoData]
  public void MapTo_NoCharacterOrShip_ThrowsArgumentException(InternalConquestReward source)
  {
    source.Character = null;
    source.CharacterId = null;
    source.Ship = null;
    source.ShipId = null;

    Assert.Throws<ArgumentException>(() => _mapper.MapTo(source));
  }

  [Theory, SwgohApiAutoData]
  public void MapFrom_ThrowsNotImplementedException(ConquestRewardDate source)
  {
    Assert.Throws<NotImplementedException>(() => _mapper.MapFrom(source));
  }
}
