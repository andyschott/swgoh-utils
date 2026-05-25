using System.Net;
using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Endpoints;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using Character = SwgohApi.Models.Earnables.Character;
using EarnableLocation = SwgohApi.Models.Earnables.EarnableLocation;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Tests.Endpoints;

public sealed class CharacterEndpointTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<ICharacterRepository> _mockCharacterRepository;
  private readonly Mock<IMarqueeRepository> _mockMarqueeRepository;
  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockEarnableLocationMapper;
  private readonly Mock<IMapper<InternalCharacter, Character>> _mockCharacterMapper;

  public CharacterEndpointTests()
  {
    _mockCharacterRepository = _mockRepository.Create<ICharacterRepository>();
    _mockMarqueeRepository = _mockRepository.Create<IMarqueeRepository>();
    _mockEarnableLocationMapper =  _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();
    _mockCharacterMapper = _mockRepository.Create<IMapper<InternalCharacter, Character>>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, SwgohApiAutoData]
  public async Task CreateCharacter_Successful(CreateCharacterRequest request,
    InternalEarnableLocation[] internalLocations,
    InternalCharacter internalCharacter,
    InternalMarquee internalMarquee,
    Character character)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacterByName(request.Name))
      .ReturnsAsync((InternalCharacter?)null);
    foreach (var (srcLocation, destLocation) in request.Locations.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    _mockCharacterRepository.Setup(repository => repository.CreateCharacter(
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

    _mockCharacterMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await CharacterEndpoints.CreateCharacter(
      request,
      _mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateCharacter_NotAMarquee_Sucessful(InternalEarnableLocation[] internalLocations,
    InternalCharacter internalCharacter,
    Character character,
    IFixture fixture)
  {
    var request = fixture.Build<CreateCharacterRequest>()
      .With(request => request.Marquee, (CharacterMarqueeRequest?)null)
      .Create();

    _mockCharacterRepository.Setup(repository => repository.GetCharacterByName(request.Name))
      .ReturnsAsync((InternalCharacter?)null);
    foreach (var (srcLocation, destLocation) in request.Locations.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(srcLocation))
        .Returns(destLocation);
    }

    _mockCharacterRepository.Setup(repository => repository.CreateCharacter(
        request.Name,
        internalLocations,
        request.IsAccelerated))
      .ReturnsAsync(internalCharacter);

    _mockCharacterMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await CharacterEndpoints.CreateCharacter(
      request,
      _mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockCharacterMapper.Object);

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
    _mockCharacterRepository.Setup(repository => repository.GetCharacterByName(request.Name))
      .ReturnsAsync(internalCharacter);

    var response = await CharacterEndpoints.CreateCharacter(
      request,
      _mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.BadRequest, problemResult.StatusCode);
  }

  [Theory, SwgohApiAutoData]
  public async Task UpdateCharacter_Successful(UpdateCharacterRequest request,
    InternalEarnableLocation[] internalLocations,
    Character character,
    IFixture fixture)
  {
    var internalCharacter = fixture.Build<InternalCharacter>()
      .With(c => c.EarnableShards, (InternalEarnableShards?)null)
      .Create();
    _mockCharacterRepository.Setup(repository => repository.GetCharacter(internalCharacter.Id))
      .ReturnsAsync(internalCharacter);

    foreach (var (src, dest) in request.Locations!.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(src))
        .Returns(dest);
    }

    _mockCharacterRepository.Setup(repository => repository.SaveCharacter(
      It.Is<InternalCharacter>(c => c.IsAccelerated == request.IsAccelerated &&
                                    c.Locations.SequenceEqual(internalLocations))))
      .Returns(Task.CompletedTask);

    _mockMarqueeRepository.Setup(repository => repository.SaveMarquee(internalCharacter.Marquee!))
      .Returns(Task.CompletedTask);

    _mockCharacterMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await CharacterEndpoints.UpdateCharacter(internalCharacter.Id,
      request,
      _mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockCharacterMapper.Object,
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
      .With(c => c.EarnableShards, (InternalEarnableShards?)null)
      .Create();

    _mockCharacterRepository.Setup(repository => repository.GetCharacter(internalCharacter.Id))
      .ReturnsAsync(internalCharacter);

    foreach (var (src, dest) in request.Locations!.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(src))
        .Returns(dest);
    }

    _mockCharacterRepository.Setup(repository => repository.SaveCharacter(
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

    _mockCharacterMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await CharacterEndpoints.UpdateCharacter(internalCharacter.Id,
      request,
      _mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockCharacterMapper.Object,
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
      .With(c => c.EarnableShards, (InternalEarnableShards?)null)
      .Create();
    var request = fixture.Build<UpdateCharacterRequest>()
      .With(request => request.Marquee, (CharacterMarqueeRequest?)null)
      .Create();

    _mockCharacterRepository.Setup(repository => repository.GetCharacter(internalCharacter.Id))
      .ReturnsAsync(internalCharacter);

    foreach (var (src, dest) in request.Locations!.Zip(internalLocations))
    {
      _mockEarnableLocationMapper.Setup(mapper => mapper.MapFrom(src))
        .Returns(dest);
    }

    _mockCharacterRepository.Setup(repository => repository.SaveCharacter(
        It.Is<InternalCharacter>(c => c.IsAccelerated == request.IsAccelerated &&
                                      c.Locations.SequenceEqual(internalLocations))))
      .Returns(Task.CompletedTask);

    _mockCharacterMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await CharacterEndpoints.UpdateCharacter(internalCharacter.Id,
      request,
      _mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockCharacterMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task UpdateCharacter_CharacterNotFound_ReturnsNotFound(string id,
    UpdateCharacterRequest request)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacter(id))
      .ReturnsAsync((InternalCharacter?)null);

    var response = await CharacterEndpoints.UpdateCharacter(id,
      request,
      _mockCharacterRepository.Object,
      _mockMarqueeRepository.Object,
      _mockCharacterMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetCharacterByName_Successful(InternalCharacter internalCharacter,
    Character character)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacterByName(internalCharacter.Name))
      .ReturnsAsync(internalCharacter);
    _mockCharacterMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await CharacterEndpoints.GetCharacterByName(internalCharacter.Name,
      _mockCharacterRepository.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.Same(character, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetCharacter_CharacterByNameNotFound_ReturnsNotFound(string name)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacterByName(name))
      .ReturnsAsync((InternalCharacter?)null);

    var response = await CharacterEndpoints.GetCharacterByName(name,
      _mockCharacterRepository.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }
}
