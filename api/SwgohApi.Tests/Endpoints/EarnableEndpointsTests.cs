using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Endpoints;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;

namespace SwgohApi.Tests.Endpoints;

public sealed class EarnableEndpointsTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IEarnableRepository<InternalCharacter>> _mockEarnableRepository;
  private readonly Mock<IMapper<InternalCharacter, Character>> _mockEarnableMapper;

  public EarnableEndpointsTests()
  {
    _mockEarnableRepository = _mockRepository.Create<IEarnableRepository<InternalCharacter>>();
    _mockEarnableMapper = _mockRepository.Create<IMapper<InternalCharacter, Character>>();
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
}
