using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;

namespace SwgohApi.Endpoints;

public static class EarnableShardsEndpoints
{
  public static WebApplication MapEarnableShardsEndpoints(this WebApplication app)
  {
    app.MapGroup("charactersForUser")
      .RequireAuthorization()
      .MapEndpoints<InternalCharacter, Character>();

    return app;
  }

  private static RouteGroupBuilder MapEndpoints<TInternal, T>(this RouteGroupBuilder builder)
  where TInternal : InternalEarnable
  where T : Earnable
  {
    builder.MapPut("/{id}", CreateOrUpdateEarnableShards<TInternal>);
    builder.MapGet(string.Empty, GetEarnablesForUser<TInternal, T>);

    return builder;
  }

  public static async Task<Results<Ok<EarnableShards>, ProblemHttpResult>> CreateOrUpdateEarnableShards<TInternal>(
    string id,
    EarnableShardsRequest request,
    IEarnableRepository<TInternal> earnableRepository,
    IEarnableShardsRepository earnableShardsRepository,
    IMapper<InternalFarmingStatus, FarmingStatus> farmingStatusMapper,
    IMapper<InternalEarnableShards, EarnableShards> earnableShardsMapper,
    HttpContext httpContext)
  where TInternal : InternalEarnable
  {
    var requestingUser = httpContext.RequestingUser;
    if (requestingUser is null)
    {
      return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden);
    }

    var earnable = await earnableRepository.GetEarnableForUser(id, requestingUser);
    if (earnable is null)
    {
      return TypedResults.Problem(statusCode: StatusCodes.Status404NotFound);
    }

    var internalFarmingStatus = farmingStatusMapper.MapFrom(request.FarmingStatus);

    var internalEarnableShards = earnable.CurrentEarnableShards;
    if (internalEarnableShards is null)
    {
      internalEarnableShards = await earnableShardsRepository.CreateEarnableShards(requestingUser,
        earnable,
        request.Shards,
        internalFarmingStatus);
    }
    else
    {
      internalEarnableShards.Shards = request.Shards;
      internalEarnableShards.FarmingStatus = internalFarmingStatus;

      await earnableShardsRepository.SaveEarnableShards(internalEarnableShards);

    }

    return TypedResults.Ok(earnableShardsMapper.MapTo(internalEarnableShards));
  }

  public static async Task<Results<Ok<IEnumerable<T>>, ProblemHttpResult>> GetEarnablesForUser<TInternal, T>(
    IEarnableRepository<TInternal> earnableRepository,
    IMapper<TInternal, T> characterMapper,
    HttpContext httpContext)
  where TInternal : InternalEarnable
  where T : Earnable
  {
    var requestingUser = httpContext.RequestingUser;
    if (requestingUser is null)
    {
      return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden);
    }

    var characters = await earnableRepository.GetEarnablesForUser(requestingUser);
    return TypedResults.Ok(characters.Select(characterMapper.MapTo));
  }
}
