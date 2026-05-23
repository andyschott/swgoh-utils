using System.Net;
using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Endpoints;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities.Customizations;
using Character = SwgohApi.Models.Earnables.Character;
using EarnableLocation = SwgohApi.Models.Earnables.EarnableLocation;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Tests.Endpoints;

public sealed class CharacterEndpointTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<ICharacterRepository> _mockCharacterRepository;
  private readonly Mock<IMapper<InternalEarnableLocation, EarnableLocation>> _mockEarnableLocationMapper;
  private readonly Mock<IMapper<InternalCharacter, Character>> _mockCharacterMapper;

  public CharacterEndpointTests()
  {
    _mockCharacterRepository = _mockRepository.Create<ICharacterRepository>();
    _mockEarnableLocationMapper =  _mockRepository.Create<IMapper<InternalEarnableLocation, EarnableLocation>>();
    _mockCharacterMapper = _mockRepository.Create<IMapper<InternalCharacter, Character>>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, AutoDomainData]
  public async Task CreateCharacter_Successful(CreateCharacterRequest request,
    InternalEarnableLocation[] internalLocations,
    InternalCharacter internalCharacter,
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
      request.IsAccelerated,
      null))
      .ReturnsAsync(internalCharacter);

    _mockCharacterMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await CharacterEndpoints.CreateCharacter(
      request,
      _mockCharacterRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Same(character, okResult.Value);
  }

  [Theory, AutoDomainData]
  public async Task CreateCharacter_CharacterAlreadyExists_ReturnsBadRequest(
    CreateCharacterRequest request,
    InternalCharacter internalCharacter)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacterByName(request.Name))
      .ReturnsAsync(internalCharacter);

    var response = await CharacterEndpoints.CreateCharacter(
      request,
      _mockCharacterRepository.Object,
      _mockEarnableLocationMapper.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.BadRequest, problemResult.StatusCode);
  }

  [Theory, AutoDomainData]
  public async Task GetCharacters_Successful(InternalCharacter[] internalCharacters,
    Character[] characters)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacters())
      .ReturnsAsync(internalCharacters);

    foreach (var (src, dest) in internalCharacters.Zip(characters))
    {
      _mockCharacterMapper.Setup(mapper => mapper.MapTo(src))
        .Returns(dest);
    }

    var response = await CharacterEndpoints.GetCharacters(
      _mockCharacterRepository.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<IEnumerable<Character>>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<IEnumerable<Character>>>(result.Result);

    Assert.Equal(characters, okResult.Value);
  }

  [Theory, AutoDomainData]
  public async Task GetCharacter_Successful(InternalCharacter internalCharacter,
    Character character)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacter(internalCharacter.Id))
      .ReturnsAsync(internalCharacter);
    _mockCharacterMapper.Setup(mapper => mapper.MapTo(internalCharacter))
      .Returns(character);

    var response = await CharacterEndpoints.GetCharacter(internalCharacter.Id,
      _mockCharacterRepository.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.Same(character, okResult.Value);
  }

  [Theory, AutoDomainData]
  public async Task GetCharacter_CharacterNotFound_ReturnsNotFound(string id)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacter(id))
      .ReturnsAsync((InternalCharacter?)null);

    var response = await CharacterEndpoints.GetCharacter(id,
      _mockCharacterRepository.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }

  [Theory, AutoDomainData]
  public async Task UpdateCharacter_Successful(InternalCharacter internalCharacter,
    UpdateCharacterRequest request,
    InternalEarnableLocation[] internalLocations,
    Character character)
  {
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
      _mockCharacterMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<Character>>(result.Result);

    Assert.Same(character, okResult.Value);
  }

  [Theory, AutoDomainData]
  public async Task UpdateCharacter_CharacterNotFound_ReturnsNotFound(string id,
    UpdateCharacterRequest request)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacter(id))
      .ReturnsAsync((InternalCharacter?)null);

    var response = await CharacterEndpoints.UpdateCharacter(id,
      request,
      _mockCharacterRepository.Object,
      _mockCharacterMapper.Object,
      _mockEarnableLocationMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }

  [Theory, AutoDomainData]
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

  [Theory, AutoDomainData]
  public async Task GetCharacter_CharacterByNameNotFound_ReturnsNotFound(string name)
  {
    _mockCharacterRepository.Setup(repository => repository.GetCharacter(name))
      .ReturnsAsync((InternalCharacter?)null);

    var response = await CharacterEndpoints.GetCharacter(name,
      _mockCharacterRepository.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<Character>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
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
