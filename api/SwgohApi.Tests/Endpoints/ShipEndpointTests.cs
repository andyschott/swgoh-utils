using System.Net;
using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Endpoints;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Tests.Endpoints;

public sealed class ShipEndpointTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IShipRepository> _mockShipRepository;
  private readonly Mock<IMarqueeRepository> _mockMarqueeRepository;
  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockEarnableLocationMapper;
  private readonly Mock<IMapper<InternalShip, Ship>> _mockShipMapper;

  public ShipEndpointTests()
  {
    _mockShipRepository = _mockRepository.Create<IShipRepository>();
    _mockMarqueeRepository = _mockRepository.Create<IMarqueeRepository>();
    _mockEarnableLocationMapper =  _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();
    _mockShipMapper = _mockRepository.Create<IMapper<InternalShip, Ship>>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, SwgohApiAutoData]
  public async Task UpdateShip_Successful(UpdateShipRequest request,
    InternalEarnableLocation[] internalLocations,
    Ship ship,
    IFixture fixture)
  {
    var internalShip = fixture.Build<InternalShip>()
      .With(c => c.EarnableShards, (InternalEarnableShards?)null)
      .Create();
    _mockShipRepository.Setup(repository => repository.GetShip(internalShip.Id))
      .ReturnsAsync(internalShip);

    foreach (var (src, dest) in request.Locations!.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(src))
        .Returns(dest);
    }

    _mockShipRepository.Setup(repository => repository.SaveShip(
        It.Is<InternalShip>(s => s.Locations.SequenceEqual(internalLocations))))
      .Returns(Task.CompletedTask);

    _mockMarqueeRepository.Setup(repository => repository.SaveMarquee(internalShip.Marquee!))
      .Returns(Task.CompletedTask);

    _mockShipMapper.Setup(mapper => mapper.MapTo(internalShip))
      .Returns(ship);

    var response = await ShipEndpoints.UpdateShip(internalShip.Id,
      request,
      _mockShipRepository.Object,
      _mockMarqueeRepository.Object,
      _mockShipMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Ship>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Ship>>(result.Result);

    Assert.Same(ship, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task UpdateShip_CreatingMarquee_Successful(UpdateShipRequest request,
    InternalEarnableLocation[] internalLocations,
    InternalMarquee internalMarquee,
    Ship ship,
    IFixture fixture)
  {
    var internalShip = fixture.Build<InternalShip>()
      .With(s => s.Marquee, (InternalMarquee?)null)
      .With(c => c.EarnableShards, (InternalEarnableShards?)null)
      .Create();

    _mockShipRepository.Setup(repository => repository.GetShip(internalShip.Id))
      .ReturnsAsync(internalShip);

    foreach (var (src, dest) in request.Locations!.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(src))
        .Returns(dest);
    }

    _mockShipRepository.Setup(repository => repository.SaveShip(
        It.Is<InternalShip>(s => s.Locations.SequenceEqual(internalLocations))))
      .Returns(Task.CompletedTask);

    _mockMarqueeRepository.Setup(repository => repository.CreateMarquee(
      internalShip,
      request.Marquee!.IntroductionDate,
      request.Marquee.MarqueeEventDate,
      request.Marquee.ShipmentDate,
      request.Marquee.FarmDate,
      null))
      .ReturnsAsync(internalMarquee);

    _mockShipMapper.Setup(mapper => mapper.MapTo(internalShip))
      .Returns(ship);

    var response = await ShipEndpoints.UpdateShip(internalShip.Id,
      request,
      _mockShipRepository.Object,
      _mockMarqueeRepository.Object,
      _mockShipMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Ship>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Ship>>(result.Result);

    Assert.Same(ship, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task UpdateShip_NotAMarquee_Successful(InternalEarnableLocation[] internalLocations,
    Ship ship,
    IFixture fixture)
  {
    var internalShip = fixture.Build<InternalShip>()
      .With(s => s.Marquee, (InternalMarquee?)null)
      .With(c => c.EarnableShards, (InternalEarnableShards?)null)
      .Create();
    var request = fixture.Build<UpdateShipRequest>()
      .With(r => r.Marquee, (ShipMarqueeRequest?)null)
      .Create();

    _mockShipRepository.Setup(repository => repository.GetShip(internalShip.Id))
      .ReturnsAsync(internalShip);

    foreach (var (src, dest) in request.Locations!.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(src))
        .Returns(dest);
    }

    _mockShipRepository.Setup(repository => repository.SaveShip(
        It.Is<InternalShip>(s => s.Locations.SequenceEqual(internalLocations))))
      .Returns(Task.CompletedTask);

    _mockShipMapper.Setup(mapper => mapper.MapTo(internalShip))
      .Returns(ship);

    var response = await ShipEndpoints.UpdateShip(internalShip.Id,
      request,
      _mockShipRepository.Object,
      _mockMarqueeRepository.Object,
      _mockShipMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Ship>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Ship>>(result.Result);

    Assert.Same(ship, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task UpdateShip_ShipNotFound_ReturnsNotFound(string id,
    UpdateShipRequest request)
  {
    _mockShipRepository.Setup(repository => repository.GetShip(id))
      .ReturnsAsync((InternalShip?)null);

    var response = await ShipEndpoints.UpdateShip(id,
      request,
      _mockShipRepository.Object,
      _mockMarqueeRepository.Object,
      _mockShipMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Ship>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }
}
