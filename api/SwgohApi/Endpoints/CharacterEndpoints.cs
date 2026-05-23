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
  public static WebApplication MapCharacterEndpoints(this WebApplication app)
  {
    var characters = app.MapGroup("/characters")
      .RequireAuthorization();

    characters.MapPost(string.Empty, CreateCharacter)
      .AddEndpointFilter<RequireAdminEndpointFilter>();

    characters.MapGet(string.Empty, GetCharacters)
      .AllowAnonymous();
    characters.MapGet("/{id}", GetCharacter)
      .AllowAnonymous();

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

  public static async Task<Results<Ok<Character>, ProblemHttpResult>> GetCharacter(string id,
    ICharacterRepository characterRepository,
    IMapper<InternalCharacter, Character> characterMapper)
  {
    var character = await characterRepository.GetCharacter(id);
    if (character is null)
    {
      return TypedResults.Problem(detail:"No character with that ID exists.",
        statusCode:(int)HttpStatusCode.NotFound);
    }

    return TypedResults.Ok(characterMapper.MapTo(character));
  }
}
