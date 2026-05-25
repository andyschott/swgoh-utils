using AutoFixture;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Tests.Mappers;

public sealed class ShipMapperTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockLocationMapper;
  private readonly Mock<IMapper<InternalMarquee, Marquee>> _mockMarqueeMapper;
  private readonly Mock<IMapper<InternalEarnableShards, EarnableShards>> _mockEarnableShardsMapper;

  private readonly ShipMapper _mapper;

  public ShipMapperTests()
  {
    _mockLocationMapper = _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();
    _mockMarqueeMapper = _mockRepository.Create<IMapper<InternalMarquee, Marquee>>();
    _mockEarnableShardsMapper = _mockRepository.Create<IMapper<InternalEarnableShards, EarnableShards>>();

    _mapper = new  ShipMapper(_mockLocationMapper.Object,
      _mockMarqueeMapper.Object,
      _mockEarnableShardsMapper.Object);
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, SwgohApiAutoData]
  public void MapTo_Successful(EarnableLocation[] destinationLocations,
    Marquee destinationMarquee,
    EarnableShards destinationEarnableShards,
    IFixture fixture)
  {
    var source = fixture.Build<InternalShip>()
      .Create();
    foreach (var (srcLocation, destLocation) in source.Locations.Zip(destinationLocations))
    {
      _mockLocationMapper.Setup(mapper => mapper.MapTo(srcLocation))
        .Returns(destLocation);
    }

    _mockMarqueeMapper.Setup(mapper => mapper.MapTo(source.Marquee!))
      .Returns(destinationMarquee);
    _mockEarnableShardsMapper.Setup(mapper => mapper.MapTo(source.EarnableShards!))
      .Returns(destinationEarnableShards);

    var result = _mapper.MapTo(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(source.Name, result.Name);
    Assert.Equal(destinationLocations, result.Locations);

    Assert.NotNull(result.Marquee);
    Assert.Same(destinationMarquee, result.Marquee);

    Assert.NotNull(result.Shards);
    Assert.Same(destinationEarnableShards, result.Shards);
  }

  [Theory, SwgohApiAutoData]
  public void MapTo_NotAMarqueeAndNoEarnableShards_Successful(EarnableLocation[] destinationLocations,
    IFixture fixture)
  {
    var source = fixture.Build<InternalShip>()
      .Without(source => source.Marquee)
      .With(c => c.EarnableShards, (InternalEarnableShards?)null)
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
    Assert.Null(result.Marquee);
    Assert.Null(result.Shards);
  }

  [Theory, SwgohApiAutoData]
  public void MapFrom_Successful(Ship source,
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

    Assert.NotNull(result.Marquee);
    Assert.Same(destinationMarquee, result.Marquee);
    Assert.Equal(source.Id, result.Marquee.ShipId);
    Assert.Null(result.Marquee.CharacterId);

    Assert.NotNull(result.EarnableShards);
    Assert.Same(destinationEarnableShards, result.EarnableShards);
    Assert.Equal(source.Id, result.EarnableShards.ShipId);
    Assert.Null(result.EarnableShards.CharacterId);
  }

  [Theory, SwgohApiAutoData]
  public void MapFrom_NotAMarqueeOrEarnableShards_Successful(InternalEarnableLocation[] destinationLocations,
    IFixture fixture)
  {
    var source = fixture.Build<Ship>()
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
    Assert.Null(result.Marquee);
    Assert.Null(result.EarnableShards);
  }
}
