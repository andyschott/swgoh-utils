using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Endpoints;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities;
using Character = SwgohApi.Models.Earnables.Character;
using EarnableShards = SwgohApi.Models.Earnables.EarnableShards;
using FarmingStatus = SwgohApi.Models.Earnables.FarmingStatus;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;

namespace SwgohApi.Tests.Endpoints;

public sealed class EarnableShardsEndpointsTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IEarnableRepository<InternalCharacter>> _mockEarnableRepository;
  private readonly Mock<IEarnableShardsRepository> _mockEarnableShardsRepository;
  private readonly Mock<IMapper<InternalFarmingStatus, FarmingStatus>> _mockFarmingStatusMapper;
  private readonly Mock<IMapper<InternalEarnableShards, EarnableShards>> _mockEarnableShardsMapper;
  private readonly Mock<IMapper<InternalCharacter, Character>> _mockCharacterMapper;

  private readonly HttpContext _httpContext = new DefaultHttpContext();

  public EarnableShardsEndpointsTests()
  {
    _mockEarnableRepository = _mockRepository.Create<IEarnableRepository<InternalCharacter>>();
    _mockEarnableShardsRepository = _mockRepository.Create<IEarnableShardsRepository>();
    _mockFarmingStatusMapper = _mockRepository.Create<IMapper<InternalFarmingStatus, FarmingStatus>>();
    _mockEarnableShardsMapper = _mockRepository.Create<IMapper<InternalEarnableShards, EarnableShards>>();
    _mockCharacterMapper = _mockRepository.Create<IMapper<InternalCharacter, Character>>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, SwgohApiAutoData]
  public async Task CreateOrUpdateEarnableShards_NoRequestingUser_ReturnsForbidden(
    string id,
    EarnableShardsRequest request)
  {
    var response = await EarnableShardsEndpoints.CreateOrUpdateEarnableShards(
      id,
      request,
      _mockEarnableRepository.Object,
      _mockEarnableShardsRepository.Object,
      _mockFarmingStatusMapper.Object,
      _mockEarnableShardsMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<EarnableShards>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal(StatusCodes.Status403Forbidden, problemResult.StatusCode);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateOrUpdateEarnableShards_EarnableNoFound_ReturnsNotFound(
    string id,
    EarnableShardsRequest request,
    User requestingUser)
  {
    _httpContext.RequestingUser = requestingUser;

    _mockEarnableRepository.Setup(repository => repository.GetEarnableForUser(id, requestingUser))
      .ReturnsAsync((InternalCharacter?)null);

    var response = await EarnableShardsEndpoints.CreateOrUpdateEarnableShards(
      id,
      request,
      _mockEarnableRepository.Object,
      _mockEarnableShardsRepository.Object,
      _mockFarmingStatusMapper.Object,
      _mockEarnableShardsMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<EarnableShards>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateOrUpdateEarnableShards_CreateSuccessful(string id,
    EarnableShardsRequest request,
    User requestingUser,
    InternalEarnableShards internalEarnableShards,
    InternalFarmingStatus internalFarmingStatus,
    EarnableShards earnableShards,
    IFixture fixture)
  {
    _httpContext.RequestingUser = requestingUser;

    var internalEarnable = fixture.Build<InternalCharacter>()
      .With(c => c.EarnableShards, [])
      .Without(c => c.Marquee)
      .Create();
    _mockEarnableRepository.Setup(repository => repository.GetEarnableForUser(id, requestingUser))
      .ReturnsAsync(internalEarnable);

    _mockFarmingStatusMapper.Setup(mapper => mapper.MapFrom(request.FarmingStatus))
      .Returns(internalFarmingStatus);

    _mockEarnableShardsRepository.Setup(repository => repository.CreateEarnableShards(requestingUser,
      internalEarnable,
      request.Shards,
      internalFarmingStatus))
      .ReturnsAsync(internalEarnableShards);

    _mockEarnableShardsMapper.Setup(mapper => mapper.MapTo(internalEarnableShards))
      .Returns(earnableShards);

    var response = await EarnableShardsEndpoints.CreateOrUpdateEarnableShards(
      id,
      request,
      _mockEarnableRepository.Object,
      _mockEarnableShardsRepository.Object,
      _mockFarmingStatusMapper.Object,
      _mockEarnableShardsMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<EarnableShards>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<EarnableShards>>(result.Result);

    Assert.Same(earnableShards, okResult.Value);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateOrUpdateEarnableShards_UpdateSuccessful(string id,
    EarnableShardsRequest request,
    User requestingUser,
    InternalEarnableShards internalEarnableShards,
    InternalFarmingStatus internalFarmingStatus,
    EarnableShards earnableShards,
    IFixture fixture)
  {
    _httpContext.RequestingUser = requestingUser;

    var internalEarnable = fixture.Build<InternalCharacter>()
      .With(c => c.EarnableShards, [internalEarnableShards])
      .Without(c => c.Marquee)
      .Create();
    _mockEarnableRepository.Setup(repository => repository.GetEarnableForUser(id, requestingUser))
      .ReturnsAsync(internalEarnable);

    _mockFarmingStatusMapper.Setup(mapper => mapper.MapFrom(request.FarmingStatus))
      .Returns(internalFarmingStatus);

    _mockEarnableShardsRepository.Setup(repository => repository.SaveEarnableShards(
        It.Is<InternalEarnableShards>(actual => actual.Shards == request.Shards &&
                                                actual.FarmingStatus == internalFarmingStatus)))
      .Returns(Task.CompletedTask);

    _mockEarnableShardsMapper.Setup(mapper => mapper.MapTo(internalEarnableShards))
      .Returns(earnableShards);

    var response = await EarnableShardsEndpoints.CreateOrUpdateEarnableShards(
      id,
      request,
      _mockEarnableRepository.Object,
      _mockEarnableShardsRepository.Object,
      _mockFarmingStatusMapper.Object,
      _mockEarnableShardsMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<EarnableShards>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<EarnableShards>>(result.Result);

    Assert.Same(earnableShards, okResult.Value);
  }

  [Fact]
  public async Task GetEarnablesForUser_NoRequestingUser_ReturnsForbidden()
  {
    var response = await EarnableShardsEndpoints.GetEarnablesForUser(
      _mockEarnableRepository.Object,
      _mockCharacterMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<IEnumerable<Character>>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal(StatusCodes.Status403Forbidden, problemResult.StatusCode);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetEarnablesForUser_Successful(User requestingUser,
    InternalCharacter[] internalEarnables,
    Character[] earnables)
  {
    _httpContext.RequestingUser = requestingUser;

    _mockEarnableRepository.Setup(repository => repository.GetEarnablesForUser(requestingUser))
      .ReturnsAsync(internalEarnables);

    foreach (var (src, dest) in internalEarnables.Zip(earnables))
    {
      _mockCharacterMapper.Setup(mapper => mapper.MapTo(src))
        .Returns(dest);
    }

    var response = await EarnableShardsEndpoints.GetEarnablesForUser(
      _mockEarnableRepository.Object,
      _mockCharacterMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<IEnumerable<Character>>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<IEnumerable<Character>>>(result.Result);

    Assert.Equal(earnables, okResult.Value);
  }
}
