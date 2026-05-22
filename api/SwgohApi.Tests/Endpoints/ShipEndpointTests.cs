using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Endpoints;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Tests.Endpoints;

public sealed class ShipEndpointTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IShipRepository> _mockShipRepository;
  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockEarnableLocationMapper;
  private readonly Mock<IMapper<InternalShip, Ship>> _mockShipMapper;

  public ShipEndpointTests()
  {
    _mockShipRepository = _mockRepository.Create<IShipRepository>();
    _mockEarnableLocationMapper =  _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();
    _mockShipMapper = _mockRepository.Create<IMapper<InternalShip, Ship>>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, AutoData]
  public async Task CreateShip_Successful(CreateShipRequest request,
    InternalEarnableLocation[] internalLocations,
    InternalShip internalShip,
    Ship ship)
  {
    _mockShipRepository.Setup(repository => repository.GetShipByName(request.Name))
      .ReturnsAsync((InternalShip?)null);
    foreach (var (srcLocation, destLocation) in request.Locations.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    _mockShipRepository.Setup(repository => repository.CreateShip(
      request.Name,
      internalLocations))
      .ReturnsAsync(internalShip);

    _mockShipMapper.Setup(mapper => mapper.MapTo(internalShip))
      .Returns(ship);

    var response = await ShipEndpoints.CreateShip(
      request,
      _mockShipRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockShipMapper.Object);

    var result = Assert.IsType<Results<Ok<Ship>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Ship>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Same(ship, okResult.Value);
  }

  [Theory, AutoData]
  public async Task CreateShip_ShipAlreadyExists_ReturnsBadRequest(
    CreateShipRequest request,
    InternalShip internalShip)
  {
    _mockShipRepository.Setup(repository => repository.GetShipByName(request.Name))
      .ReturnsAsync(internalShip);

    var response = await ShipEndpoints.CreateShip(
      request,
      _mockShipRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockShipMapper.Object);

    var result = Assert.IsType<Results<Ok<Ship>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.BadRequest, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task GetShips_Successful(InternalShip[] internalShips,
    Ship[] ships)
  {
    _mockShipRepository.Setup(repository => repository.GetShips())
      .ReturnsAsync(internalShips);

    foreach (var (src, dest) in internalShips.Zip(ships))
    {
      _mockShipMapper.Setup(mapper => mapper.MapTo(src))
        .Returns(dest);
    }

    var response = await ShipEndpoints.GetShips(
      _mockShipRepository.Object,
      _mockShipMapper.Object);

    var result = Assert.IsType<Results<Ok<IEnumerable<Ship>>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<IEnumerable<Ship>>>(result.Result);

    Assert.Equal(ships, okResult.Value);
  }
}
