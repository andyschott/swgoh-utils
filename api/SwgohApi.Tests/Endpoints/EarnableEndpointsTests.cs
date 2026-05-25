using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Endpoints;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Tests.Endpoints;

public sealed class EarnableEndpointsTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IEarnableRepository<InternalCharacter>> _mockEarnableRepository;
  private readonly Mock<IMapper<InternalCharacter, Character>> _mockEarnableMapper;
  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockEarnableLocationMapper;
  private readonly Mock<IMarqueeRepository> _mockMarqueeRepository;

  public EarnableEndpointsTests()
  {
    _mockEarnableRepository = _mockRepository.Create<IEarnableRepository<InternalCharacter>>();
    _mockEarnableMapper = _mockRepository.Create<IMapper<InternalCharacter, Character>>();
    _mockEarnableLocationMapper =  _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();
    _mockMarqueeRepository = _mockRepository.Create<IMarqueeRepository>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, SwgohApiAutoData]
  public async Task GetCharacters_Successful(InternalCharacter[] internalCharacters,
    Character[] characters)
  {
    _mockEarnableRepository.Setup(repository => repository.GetEarnables())
      .ReturnsAsync(internalCharacters);

    foreach (var (src, dest) in internalCharacters.Zip(characters))
    {
      _mockEarnableMapper.Setup(mapper => mapper.MapTo(src))
        .Returns(dest);
    }

    var response = await EarnableEndpoints.GetEarnables(
      _mockEarnableRepository.Object,
      _mockEarnableMapper.Object);

    var result = Assert.IsType<Results<Ok<IEnumerable<Character>>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<IEnumerable<Character>>>(result.Result);

    Assert.Equal(characters, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetEarnable_Successful(InternalCharacter internalCharacter,
    Character character)
  {
    _mockEarnableRepository.Setup(repository => repository.GetEarnable(internalCharacter.Id))
      .ReturnsAsync(internalCharacter);
    _mockEarnableMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await EarnableEndpoints.GetEarnable(internalCharacter.Id,
      _mockEarnableRepository.Object,
      _mockEarnableMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetEarnable_EarnableNotFound_ReturnsNotFound(string id)
  {
    _mockEarnableRepository.Setup(repository => repository.GetEarnable(id))
      .ReturnsAsync((InternalCharacter?)null);

    var response = await EarnableEndpoints.GetEarnable(id,
      _mockEarnableRepository.Object,
      _mockEarnableMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetEarnableByName_Successful(InternalCharacter internalCharacter,
    Character character)
  {
    _mockEarnableRepository.Setup(repository => repository.GetEarnableByName(internalCharacter.Name))
      .ReturnsAsync(internalCharacter);
    _mockEarnableMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await EarnableEndpoints.GetEarnableByName(internalCharacter.Name,
      _mockEarnableRepository.Object,
      _mockEarnableMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetEarnable_EarnableByNameNotFound_ReturnsNotFound(string name)
  {
    _mockEarnableRepository.Setup(repository => repository.GetEarnableByName(name))
      .ReturnsAsync((InternalCharacter?)null);

    var response = await EarnableEndpoints.GetEarnableByName(name,
      _mockEarnableRepository.Object,
      _mockEarnableMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateCharacter_Successful(CreateCharacterRequest request,
    InternalEarnableLocation[] internalLocations,
    InternalCharacter internalCharacter,
    InternalMarquee internalMarquee,
    Character character)
  {
    var mockCharacterRepository = _mockRepository.Create<ICharacterRepository>();
    mockCharacterRepository.Setup(repository => repository.GetEarnableByName(request.Name))
      .ReturnsAsync((InternalCharacter?)null);
    foreach (var (srcLocation, destLocation) in request.Locations.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    mockCharacterRepository.Setup(repository => repository.CreateCharacter(
        request.Name,
        internalLocations,
        request.IsAccelerated))
      .ReturnsAsync(internalCharacter);

    _mockMarqueeRepository.Setup(repository => repository.CreateMarquee(internalCharacter,
        request.Marquee!.IntroductionDate,
        request.Marquee.MarqueeEventDate,
        request.Marquee.ShipmentDate,
        request.Marquee.FarmDate,
        request.Marquee.AccelerationDate))
      .ReturnsAsync(internalMarquee);

    _mockEarnableMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await EarnableEndpoints.CreateCharacter(
      request,
      mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockEarnableMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateCharacter_NotAMarquee_Successful(InternalEarnableLocation[] internalLocations,
    InternalCharacter internalCharacter,
    Character character,
    IFixture fixture)
  {
    var request = fixture.Build<CreateCharacterRequest>()
      .With(request => request.Marquee, (CharacterMarqueeRequest?)null)
      .Create();

    var mockCharacterRepository = _mockRepository.Create<ICharacterRepository>();
    mockCharacterRepository.Setup(repository => repository.GetEarnableByName(request.Name))
      .ReturnsAsync((InternalCharacter?)null);
    foreach (var (srcLocation, destLocation) in request.Locations.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    mockCharacterRepository.Setup(repository => repository.CreateCharacter(
        request.Name,
        internalLocations,
        request.IsAccelerated))
      .ReturnsAsync(internalCharacter);

    _mockEarnableMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await EarnableEndpoints.CreateCharacter(
      request,
      mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockEarnableMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateCharacter_CharacterAlreadyExists_ReturnsBadRequest(
    CreateCharacterRequest request,
    InternalCharacter internalCharacter)
  {
    var mockCharacterRepository = _mockRepository.Create<ICharacterRepository>();
    mockCharacterRepository.Setup(repository => repository.GetEarnableByName(request.Name))
      .ReturnsAsync(internalCharacter);

    var response = await EarnableEndpoints.CreateCharacter(
      request,
      mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockEarnableMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateShip_Successful(CreateShipRequest request,
    InternalEarnableLocation[] internalLocations,
    InternalShip internalShip,
    InternalMarquee internalMarquee,
    Ship ship)
  {
    var mockShipRepository = _mockRepository.Create<IShipRepository>();
    mockShipRepository.Setup(repository => repository.GetEarnableByName(request.Name))
      .ReturnsAsync((InternalShip?)null);
    foreach (var (srcLocation, destLocation) in request.Locations.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    mockShipRepository.Setup(repository => repository.CreateShip(
        request.Name,
        internalLocations))
      .ReturnsAsync(internalShip);

    _mockMarqueeRepository.Setup(repository => repository.CreateMarquee(internalShip,
        request.Marquee!.IntroductionDate,
        request.Marquee.MarqueeEventDate,
        request.Marquee.ShipmentDate,
        request.Marquee.FarmDate,
        null))
      .ReturnsAsync(internalMarquee);

    var mockShipMapper = _mockRepository.Create<IMapper<InternalShip, Ship>>();
    mockShipMapper.Setup(mapper => mapper.MapTo(internalShip))
      .Returns(ship);

    var response = await EarnableEndpoints.CreateShip(
      request,
      mockShipRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableLocationMapper.Object,
      mockShipMapper.Object);

    var result = Assert.IsType<Results<Ok<Ship>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Ship>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Same(ship, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateShip_NotAMarquee_Successful(InternalEarnableLocation[] internalLocations,
    InternalShip internalShip,
    Ship ship,
    IFixture fixture)
  {
    var mockShipRepository = _mockRepository.Create<IShipRepository>();
    var request = fixture.Build<CreateShipRequest>()
      .With(r => r.Marquee, (MarqueeRequest?)null)
      .Create();

    mockShipRepository.Setup(repository => repository.GetEarnableByName(request.Name))
      .ReturnsAsync((InternalShip?)null);
    foreach (var (srcLocation, destLocation) in request.Locations.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    mockShipRepository.Setup(repository => repository.CreateShip(
        request.Name,
        internalLocations))
      .ReturnsAsync(internalShip);

    var mockShipMapper = _mockRepository.Create<IMapper<InternalShip, Ship>>();
    mockShipMapper.Setup(mapper => mapper.MapTo(internalShip))
      .Returns(ship);

    var response = await EarnableEndpoints.CreateShip(
      request,
      mockShipRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableLocationMapper.Object,
      mockShipMapper.Object);

    var result = Assert.IsType<Results<Ok<Ship>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Ship>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Same(ship, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateShip_ShipAlreadyExists_ReturnsBadRequest(
    CreateShipRequest request,
    InternalShip internalShip)
  {
    var mockShipRepository = _mockRepository.Create<IShipRepository>();
    mockShipRepository.Setup(repository => repository.GetEarnableByName(request.Name))
      .ReturnsAsync(internalShip);

    var mockShipMapper = _mockRepository.Create<IMapper<InternalShip, Ship>>();
    var response = await EarnableEndpoints.CreateShip(
      request,
      mockShipRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableLocationMapper.Object,
      mockShipMapper.Object);

    var result = Assert.IsType<Results<Ok<Ship>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
  }

  [Theory, SwgohApiAutoData]
  public async Task UpdateCharacter_Successful(UpdateCharacterRequest request,
    InternalEarnableLocation[] internalLocations,
    Character character,
    IFixture fixture)
  {
    var internalCharacter = fixture.Build<InternalCharacter>()
      .With(c => c.EarnableShards, [])
      .Create();
    _mockEarnableRepository.Setup(repository => repository.GetEarnable(internalCharacter.Id))
      .ReturnsAsync(internalCharacter);

    foreach (var (src, dest) in request.Locations!.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(src))
        .Returns(dest);
    }

    _mockEarnableRepository.Setup(repository => repository.SaveEarnable(
        It.Is<InternalCharacter>(c => c.IsAccelerated == request.IsAccelerated &&
                                      c.Locations.SequenceEqual(internalLocations))))
      .Returns(Task.CompletedTask);

    _mockMarqueeRepository.Setup(repository => repository.SaveMarquee(internalCharacter.Marquee!))
      .Returns(Task.CompletedTask);

    _mockEarnableMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await EarnableEndpoints.UpdateCharacter(internalCharacter.Id,
      request,
      _mockEarnableRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task UpdateCharacter_CreatingMarquee_Successful(UpdateCharacterRequest request,
    InternalEarnableLocation[] internalLocations,
    InternalMarquee internalMarquee,
    Character character,
    IFixture fixture)
  {
    var internalCharacter = fixture.Build<InternalCharacter>()
      .With(c => c.Marquee, (InternalMarquee?)null)
      .With(c => c.EarnableShards, [])
      .Create();

    _mockEarnableRepository.Setup(repository => repository.GetEarnable(internalCharacter.Id))
      .ReturnsAsync(internalCharacter);

    foreach (var (src, dest) in request.Locations!.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(src))
        .Returns(dest);
    }

    _mockEarnableRepository.Setup(repository => repository.SaveEarnable(
        It.Is<InternalCharacter>(c => c.IsAccelerated == request.IsAccelerated &&
                                      c.Locations.SequenceEqual(internalLocations))))
      .Returns(Task.CompletedTask);

    _mockMarqueeRepository.Setup(repository => repository.CreateMarquee(
        internalCharacter,
        request.Marquee!.IntroductionDate,
        request.Marquee.MarqueeEventDate,
        request.Marquee.ShipmentDate,
        request.Marquee.FarmDate,
        request.Marquee.AccelerationDate))
      .ReturnsAsync(internalMarquee);

    _mockEarnableMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await EarnableEndpoints.UpdateCharacter(internalCharacter.Id,
      request,
      _mockEarnableRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task UpdateCharacter_NotAMarquee_Successful(InternalEarnableLocation[] internalLocations,
    Character character,
    IFixture fixture)
  {
    var internalCharacter = fixture.Build<InternalCharacter>()
      .With(c => c.Marquee, (InternalMarquee?)null)
      .With(c => c.EarnableShards, [])
      .Create();
    var request = fixture.Build<UpdateCharacterRequest>()
      .With(request => request.Marquee, (CharacterMarqueeRequest?)null)
      .Create();

    _mockEarnableRepository.Setup(repository => repository.GetEarnable(internalCharacter.Id))
      .ReturnsAsync(internalCharacter);

    foreach (var (src, dest) in request.Locations!.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(src))
        .Returns(dest);
    }

    _mockEarnableRepository.Setup(repository => repository.SaveEarnable(
        It.Is<InternalCharacter>(c => c.IsAccelerated == request.IsAccelerated &&
                                      c.Locations.SequenceEqual(internalLocations))))
      .Returns(Task.CompletedTask);

    _mockEarnableMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await EarnableEndpoints.UpdateCharacter(internalCharacter.Id,
      request,
      _mockEarnableRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task UpdateCharacter_CharacterNotFound_ReturnsNotFound(string id,
    UpdateCharacterRequest request)
  {
    _mockEarnableRepository.Setup(repository => repository.GetEarnable(id))
      .ReturnsAsync((InternalCharacter?)null);

    var response = await EarnableEndpoints.UpdateCharacter(id,
      request,
      _mockEarnableRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
  }
}
