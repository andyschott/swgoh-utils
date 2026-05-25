using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;

namespace SwgohApi.Endpoints;

public static class EarnableShardsEndpoints
{
  public static WebApplication MapEarnableShardsEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("charactersForUser")
      .RequireAuthorization();

    group.MapPut("/{characterId}", CreateOrUpdateEarnableShards);
    group.MapGet(string.Empty, GetCharactersForUser);

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

    var character = await characterRepository.GetCharacterForUser(characterId, requestingUser);
    if (character is null)
    {
      return TypedResults.Problem(statusCode: StatusCodes.Status404NotFound);
    }

    var internalFarmingStatus = farmingStatusMapper.MapFrom(request.FarmingStatus);
    InternalEarnableShards earnableShards;
    if (character.EarnableShards is null)
    {
      earnableShards = await earnableShardsRepository.CreateEarnableShards(requestingUser,
        character,
        request.Shards,
        internalFarmingStatus);
    }
    else
    {
      character.EarnableShards.Shards = request.Shards;
      character.EarnableShards.FarmingStatus = internalFarmingStatus;

      await earnableShardsRepository.SaveEarnableShards(character.EarnableShards);

      earnableShards = character.EarnableShards;
    }

    return TypedResults.Ok(earnableShardsMapper.MapTo(earnableShards));
  }

  public static async Task<Results<Ok<IEnumerable<Character>>, ProblemHttpResult>> GetCharactersForUser(
    ICharacterRepository characterRepository,
    IMapper<InternalCharacter, Character> characterMapper,
    HttpContext httpContext)
  {
    var requestingUser = httpContext.RequestingUser;
    if (requestingUser is null)
    {
      return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden);
    }

    var characters = await characterRepository.GetCharactersForUser(requestingUser);
    return TypedResults.Ok(characters.Select(characterMapper.MapTo));
  }
}
