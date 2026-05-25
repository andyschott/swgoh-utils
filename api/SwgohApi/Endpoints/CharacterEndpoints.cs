using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Extensions;
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

    characters.MapPut("/{id}",  UpdateCharacter)
      .RequireAdmin();

    return app;
  }

  public static async Task<Results<Ok<Character>, ProblemHttpResult>> UpdateCharacter(string id,
    UpdateCharacterRequest request,
    ICharacterRepository characterRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalCharacter, Character> characterMapper,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper)
  {
    var internalCharacter = await characterRepository.GetCharacter(id);
    if (internalCharacter is null)
    {
      return TypedResults.Problem(detail:"No character with that ID exists.",
        statusCode:(int)HttpStatusCode.NotFound);
    }

    if (request.Locations is not null)
    {
      internalCharacter.Locations = request.Locations.Select(earnableLocationMapper.MapFrom)
        .ToList();
    }

    if (request.IsAccelerated is not null)
    {
      internalCharacter.IsAccelerated = request.IsAccelerated.Value;
    }

    await characterRepository.SaveCharacter(internalCharacter);

    if (request.Marquee is not null)
    {
      if (internalCharacter.Marquee is not null)
      {
        internalCharacter.Marquee.IntroductionDate = request.Marquee.IntroductionDate;
        internalCharacter.Marquee.MarqueeEventDate = request.Marquee.MarqueeEventDate;
        internalCharacter.Marquee.ShipmentDate = request.Marquee.ShipmentDate;
        internalCharacter.Marquee.FarmDate = request.Marquee.FarmDate;
        internalCharacter.Marquee.AccelerationDate = request.Marquee.AccelerationDate;

        await marqueeRepository.SaveMarquee(internalCharacter.Marquee);
      }
      else
      {
        internalCharacter.Marquee = await marqueeRepository.CreateMarquee(
          internalCharacter,
          request.Marquee.IntroductionDate,
          request.Marquee.MarqueeEventDate,
          request.Marquee.ShipmentDate,
          request.Marquee.FarmDate,
          request.Marquee.AccelerationDate);
      }
    }

    return TypedResults.Ok(characterMapper.MapTo(internalCharacter));
  }
}
