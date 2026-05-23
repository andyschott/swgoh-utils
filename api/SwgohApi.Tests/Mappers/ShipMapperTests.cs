using AutoFixture;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities.Customizations;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Tests.Mappers;

public sealed class ShipMapperTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockLocationMapper;
  private readonly Mock<IMapper<InternalMarquee, Marquee>> _mockMarqueeMapper;

  private readonly ShipMapper _mapper;

  public ShipMapperTests()
  {
    _mockLocationMapper = _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();
    _mockMarqueeMapper = _mockRepository.Create<IMapper<InternalMarquee, Marquee>>();

    _mapper = new  ShipMapper(_mockLocationMapper.Object,
      _mockMarqueeMapper.Object);
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, AutoDomainData]
  public void MapTo_Successful(InternalShip source,
    EarnableLocation[] destinationLocations,
    Marquee destinationMarquee)
  {
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

    Assert.NotNull(result.Marquee);
    Assert.Equal(destinationMarquee.Id, result.Marquee.Id);
    Assert.Equal(destinationMarquee.IntroductionDate, result.Marquee.IntroductionDate);
    Assert.Equal(destinationMarquee.MarqueeEventDate, result.Marquee.MarqueeEventDate);
    Assert.Equal(destinationMarquee.ShipmentDate, result.Marquee.ShipmentDate);
    Assert.Equal(destinationMarquee.FarmDate, result.Marquee.FarmDate);
    Assert.Equal(destinationMarquee.AccelerationDate, result.Marquee.AccelerationDate);
  }

  [Theory, AutoDomainData]
  public void MapTo_NotAMarquee_Successful(EarnableLocation[] destinationLocations,
    IFixture fixture)
  {
    var source = fixture.Build<InternalShip>()
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
    Assert.Null(result.Marquee);
  }

  [Theory, AutoDomainData]
  public void MapFrom_Successful(Ship source,
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

    Assert.NotNull(result.Marquee);
    Assert.Equal(destinationMarquee.Id, result.Marquee.Id);
    Assert.Equal(destinationMarquee.IntroductionDate, result.Marquee.IntroductionDate);
    Assert.Equal(destinationMarquee.MarqueeEventDate,  result.Marquee.MarqueeEventDate);
    Assert.Equal(destinationMarquee.ShipmentDate, result.Marquee.ShipmentDate);
    Assert.Equal(destinationMarquee.FarmDate, result.Marquee.FarmDate);
    Assert.Equal(destinationMarquee.AccelerationDate, result.Marquee.AccelerationDate);
    Assert.Equal(source.Id, result.Marquee.ShipId);
    Assert.Null(result.Marquee.CharacterId);
  }

  [Theory, AutoDomainData]
  public void MapFrom_NotAMarquee_Successful(InternalEarnableLocation[] destinationLocations,
    IFixture fixture)
  {
    var source = fixture.Build<Ship>()
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
    Assert.Null(result.Marquee);
  }

  class AutoDomainDataAttribute : AutoDataAttribute
  {
    public AutoDomainDataAttribute()
      : base(Customize)
    {
    }

    private static IFixture Customize()
    {
      var fixture = new Fixture();

      fixture.Customize(new MarqueeCustomization());

      return fixture;
    }
  }
}
