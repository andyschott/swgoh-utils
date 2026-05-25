using AutoFixture;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Tests.Mappers;

public sealed class CharacterMapperTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockLocationMapper;
  private readonly Mock<IMapper<InternalMarquee, Marquee>> _mockMarqueeMapper;
  private readonly Mock<IMapper<InternalEarnableShards, EarnableShards>> _mockEarnableShardsMapper;

  private readonly CharacterMapper _mapper;

  public CharacterMapperTests()
  {
    _mockLocationMapper = _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();
    _mockMarqueeMapper = _mockRepository.Create<IMapper<InternalMarquee, Marquee>>();
    _mockEarnableShardsMapper = _mockRepository.Create<IMapper<InternalEarnableShards, EarnableShards>>();

    _mapper = new CharacterMapper(_mockLocationMapper.Object,
      _mockMarqueeMapper.Object,
      _mockEarnableShardsMapper.Object);
  }

  public void Dispose() =>  _mockRepository.VerifyAll();

  [Theory, SwgohApiAutoData]
  public void MapTo_Successful(EarnableLocation[] destinationLocations,
    Marquee destinationMarquee,
    EarnableShards destinationEarnableShards,
    IFixture fixture)
  {
    var source = fixture.Build<InternalCharacter>()
      .With(es => es.EarnableShards, [
        fixture.Create<InternalEarnableShards>()
      ])
      .Create();
    foreach (var (srcLocation, destLocation) in source.Locations.Zip(destinationLocations))
    {
      _mockLocationMapper.Setup(mapper => mapper.MapTo(srcLocation))
        .Returns(destLocation);
    }

    _mockMarqueeMapper.Setup(mapper => mapper.MapTo(source.Marquee!))
      .Returns(destinationMarquee);
    _mockEarnableShardsMapper.Setup(mapper => mapper.MapTo(source.EarnableShards[0]))
      .Returns(destinationEarnableShards);

    var result = _mapper.MapTo(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(source.Name, result.Name);
    Assert.Equal(destinationLocations, result.Locations);
    Assert.Equal(source.IsAccelerated, result.IsAccelerated);

    Assert.NotNull(result.Marquee);
    Assert.Same(destinationMarquee, result.Marquee);

    Assert.NotNull(result.Shards);
    Assert.Same(destinationEarnableShards, result.Shards);
  }

  [Theory, SwgohApiAutoData]
  public void MapTo_NotAMarqueeAndNoEarnableShards_Successful(EarnableLocation[] destinationLocations,
    IFixture fixture)
  {
    var source = fixture.Build<InternalCharacter>()
      .Without(source => source.Marquee)
      .With(c => c.EarnableShards, [])
      .Create();
    foreach (var (srcLocation, destLocation) in source.Locations.Zip(destinationLocations))
    {
      _mockLocationMapper.Setup(mapper => mapper.MapTo(srcLocation))
        .Returns(destLocation);
    }

    var result = _mapper.MapTo(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(source.Name, result.Name);
    Assert.Equal(destinationLocations, result.Locations);
    Assert.Equal(source.IsAccelerated, result.IsAccelerated);
    Assert.Null(result.Marquee);
    Assert.Null(result.Shards);
  }

  [Theory, SwgohApiAutoData]
  public void MapFrom_Successful(Character source,
    InternalEarnableLocation[] destinationLocations,
    InternalMarquee destinationMarquee,
    InternalEarnableShards destinationEarnableShards)
  {
    foreach (var (srcLocation, destLocation) in source.Locations.Zip(destinationLocations))
    {
      _mockLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    _mockMarqueeMapper.Setup(mapper => mapper.MapFrom(source.Marquee!))
      .Returns(destinationMarquee);
    _mockEarnableShardsMapper.Setup(mapper => mapper.MapFrom(source.Shards!))
      .Returns(destinationEarnableShards);

    var result = _mapper.MapFrom(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(source.Name, result.Name);
    Assert.Equal(destinationLocations, result.Locations);
    Assert.Equal(source.IsAccelerated, result.IsAccelerated);

    Assert.NotNull(result.Marquee);
    Assert.Same(destinationMarquee, result.Marquee);
    Assert.Equal(source.Id, result.Marquee.CharacterId);
    Assert.Null(result.Marquee.ShipId);

    var actualEarnableShards = Assert.Single(result.EarnableShards);
    Assert.Equal(source.Id, actualEarnableShards.CharacterId);
    Assert.Null(actualEarnableShards.ShipId);
  }

  [Theory, SwgohApiAutoData]
  public void MapFrom_NotAMarqueeOrEarnableShards_Successful(InternalEarnableLocation[] destinationLocations,
    IFixture fixture)
  {
    var source = fixture.Build<Character>()
      .With(source => source.Marquee, (Marquee?)null)
      .With(source => source.Shards, (EarnableShards?)null)
      .Create();

    foreach (var (srcLocation, destLocation) in source.Locations.Zip(destinationLocations))
    {
      _mockLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    var result = _mapper.MapFrom(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(source.Name, result.Name);
    Assert.Equal(destinationLocations, result.Locations);
    Assert.Equal(source.IsAccelerated, result.IsAccelerated);
    Assert.Null(result.Marquee);
    Assert.Empty(result.EarnableShards);
  }
}
