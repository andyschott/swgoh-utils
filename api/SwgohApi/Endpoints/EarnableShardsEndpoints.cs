using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;

namespace SwgohApi.Endpoints;

public static class EarnableShardsEndpoints
{
  public static WebApplication MapEarnableShardsEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("characters/{characterId}")
      .RequireAuthorization();

    group.MapPut("earnableShards", CreateOrUpdateEarnableShards);

    return app;
  }

  public static async Task<Results<Ok<EarnableShards>, ProblemHttpResult>> CreateOrUpdateEarnableShards(
    string characterId,
    EarnableShardsRequest request,
    ICharacterRepository characterRepository,
    IEarnableShardsRepository earnableShardsRepository,
    IMapper<InternalFarmingStatus, FarmingStatus> farmingStatusMapper,
    IMapper<InternalEarnableShards, EarnableShards> earnableShardsMapper,
    HttpContext httpContext)
  {
    var requestingUser = httpContext.RequestingUser;
    if (requestingUser is null)
    {
      return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden);
    }

    var character = await characterRepository.GetCharacter(characterId);
    if (character is null)
    {
      return TypedResults.Problem(statusCode: StatusCodes.Status404NotFound);
    }

    var earnableShards = await earnableShardsRepository.CreateEarnableShards(requestingUser,
      character,
      request.Shards,
      farmingStatusMapper.MapFrom(request.FarmingStatus));

    return TypedResults.Ok(earnableShardsMapper.MapTo(earnableShards));
  }
}
