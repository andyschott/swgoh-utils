using AutoFixture;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Tests.Mappers;

public sealed class ShipMapperTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockLocationMapper;

  private readonly ShipMapper _mapper;

  public ShipMapperTests()
  {
    _mockLocationMapper = _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();

    _mapper = new  ShipMapper(_mockLocationMapper.Object);
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, AutoDomainData]
  public void MapTo_Successful(InternalShip source,
    EarnableLocation[] destinationLocations)
  {
    foreach (var (srcLocation, destLocation) in source.Locations.Zip(destinationLocations))
    {
      _mockLocationMapper.Setup(mapper => mapper.MapTo(srcLocation))
        .Returns(destLocation);
    }

    var result = _mapper.MapTo(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(source.Name, result.Name);
    Assert.Equal(destinationLocations, result.Locations);
  }

  [Theory, AutoDomainData]
  public void MapFrom_Successful(Ship source,
    InternalEarnableLocation[] destinationLocations)
  {
    foreach (var (srcLocation, destLocation) in source.Locations.Zip(destinationLocations))
    {
      _mockLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    var result = _mapper.MapFrom(source);

    Assert.Equal(source.Id, result.Id);
    Assert.Equal(source.Name, result.Name);
    Assert.Equal(destinationLocations, result.Locations);
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

      fixture.Customize<InternalShip>(composer => composer
        .Without(s => s.Marquee));

      return fixture;
    }
  }
}
