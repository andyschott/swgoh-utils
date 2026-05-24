using AutoFixture;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Tests.Mappers;

public sealed class CharacterMapperTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockLocationMapper;
  private readonly Mock<IMapper<InternalMarquee, Marquee>> _mockMarqueeMapper;

  private readonly CharacterMapper _mapper;

  public CharacterMapperTests()
  {
    _mockLocationMapper = _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();
    _mockMarqueeMapper = _mockRepository.Create<IMapper<InternalMarquee, Marquee>>();

    _mapper = new CharacterMapper(_mockLocationMapper.Object,
      _mockMarqueeMapper.Object);
  }

  public void Dispose() =>  _mockRepository.VerifyAll();

  [Theory, SwgohApiAutoData]
  public void MapTo_Successful(EarnableLocation[] destinationLocations,
    Marquee destinationMarquee,
    IFixture fixture)
  {
    var source = fixture.Build<InternalCharacter>()
      .Create();
    foreach (var (srcLocation, destLocation) in source.Locations.Zip(destinationLocations))
    {
      _mockLocationMapper.Setup(mapper => mapper.MapTo(srcLocation))
        .Returns(destLocation);
    }

    _mockMarqueeMapper.Setup(mapper => mapper.MapTo(source.Marquee!))
      .Returns(destinationMarquee);

    var result = _mapper.MapTo(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(source.Name, result.Name);
    Assert.Equal(destinationLocations, result.Locations);
    Assert.Equal(source.IsAccelerated, result.IsAccelerated);

    Assert.NotNull(result.Marquee);
    Assert.Equal(destinationMarquee.Id, result.Marquee.Id);
    Assert.Equal(destinationMarquee.IntroductionDate, result.Marquee.IntroductionDate);
    Assert.Equal(destinationMarquee.MarqueeEventDate, result.Marquee.MarqueeEventDate);
    Assert.Equal(destinationMarquee.ShipmentDate, result.Marquee.ShipmentDate);
    Assert.Equal(destinationMarquee.FarmDate, result.Marquee.FarmDate);
    Assert.Equal(destinationMarquee.AccelerationDate, result.Marquee.AccelerationDate);
  }

  [Theory, SwgohApiAutoData]
  public void MapTo_NotAMarquee_Successful(EarnableLocation[] destinationLocations,
    IFixture fixture)
  {
    var source = fixture.Build<InternalCharacter>()
      .Without(source => source.Marquee)
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
  }

  [Theory, SwgohApiAutoData]
  public void MapFrom_Successful(Character source,
    InternalEarnableLocation[] destinationLocations,
    InternalMarquee destinationMarquee)
  {
    foreach (var (srcLocation, destLocation) in source.Locations.Zip(destinationLocations))
    {
      _mockLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    _mockMarqueeMapper.Setup(mapper => mapper.MapFrom(source.Marquee!))
      .Returns(destinationMarquee);

    var result = _mapper.MapFrom(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(source.Name, result.Name);
    Assert.Equal(destinationLocations, result.Locations);
    Assert.Equal(source.IsAccelerated, result.IsAccelerated);

    Assert.NotNull(result.Marquee);
    Assert.Equal(destinationMarquee.Id, result.Marquee.Id);
    Assert.Equal(destinationMarquee.IntroductionDate, result.Marquee.IntroductionDate);
    Assert.Equal(destinationMarquee.MarqueeEventDate,  result.Marquee.MarqueeEventDate);
    Assert.Equal(destinationMarquee.ShipmentDate, result.Marquee.ShipmentDate);
    Assert.Equal(destinationMarquee.FarmDate, result.Marquee.FarmDate);
    Assert.Equal(destinationMarquee.AccelerationDate, result.Marquee.AccelerationDate);
    Assert.Equal(source.Id, result.Marquee.CharacterId);
    Assert.Null(result.Marquee.ShipId);
  }

  [Theory, SwgohApiAutoData]
  public void MapFrom_NotAMarquee_Successful(InternalEarnableLocation[] destinationLocations,
    IFixture fixture)
  {
    var source = fixture.Build<Character>()
      .With(source => source.Marquee, (Marquee?)null)
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
  }
}
