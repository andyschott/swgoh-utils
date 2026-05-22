using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Filters;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Endpoints;

public static class CharacterEndpoints
{
  public static WebApplication MapCharacterEndpoints(this WebApplication app,
    bool allowCreation)
  {
    var characters = app.MapGroup("/characters")
      .RequireAuthorization();

    characters.MapPost(string.Empty, CreateCharacter)
      .AddEndpointFilter<RequireAdminEndpointFilter>();

    characters.MapGet(string.Empty,  GetCharacters);

    return app;
  }

  public static async Task<Results<Ok<Character>, ProblemHttpResult>> CreateCharacter(
    CreateCharacterRequest request,
    ICharacterRepository characterRepository,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalCharacter, Character> characterMapper)
  {
    var existingCharacter = await characterRepository.GetCharacterByName(request.Name);
    if (existingCharacter is not null)
    {
      return TypedResults.Problem("A character with that name already exists.",
        statusCode:(int)HttpStatusCode.BadRequest);
    }

    var locations = request.Locations.Select(earnableLocationMapper.MapFrom)
      .ToList();

    var character = await characterRepository.CreateCharacter(request.Name,
      locations,
      request.IsAccelerated);

    return TypedResults.Ok(characterMapper.MapTo(character));
  }

  public static async Task<Results<Ok<IEnumerable<Character>>, ProblemHttpResult>> GetCharacters(
    ICharacterRepository characterRepository,
    IMapper<InternalCharacter, Character> characterMapper)
  {
    var characters = await characterRepository.GetCharacters();

    return TypedResults.Ok(characters.Select(characterMapper.MapTo));
  }
}
