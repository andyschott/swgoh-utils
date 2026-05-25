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
  private readonly Mock<IMapper<InternalCharacter, Character>> _mockCharacterMapper;

  public EarnableEndpointsTests()
  {
    _mockEarnableRepository = _mockRepository.Create<IEarnableRepository<InternalCharacter>>();
    _mockCharacterMapper = _mockRepository.Create<IMapper<InternalCharacter, Character>>();
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
      _mockCharacterMapper.Setup(mapper => mapper.MapTo(src))
        .Returns(dest);
    }

    var response = await EarnableEndpoints.GetEarnables(
      _mockEarnableRepository.Object,
      _mockCharacterMapper.Object);

    var result = Assert.IsType<Results<Ok<IEnumerable<Character>>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<IEnumerable<Character>>>(result.Result);

    Assert.Equal(characters, okResult.Value);
  }
}
