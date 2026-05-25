using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Endpoints;

public static class EarnableEndpoints
{
  public static WebApplication MapEarnableEndpoints(this WebApplication app)
  {
    var characters = app.MapGroup("/characters")
      .RequireAuthorization()
      .MapEndpoints<InternalCharacter, Character>();
    characters.MapPost(string.Empty, CreateCharacter)
      .RequireAdmin();
    characters.MapPut("/{id}", UpdateCharacter)
      .RequireAdmin();

    var ships = app.MapGroup("/ships")
      .RequireAuthorization()
      .MapEndpoints<InternalShip, Ship>();
    ships.MapPost(string.Empty, CreateShip)
      .RequireAdmin();
    ships.MapPut("/{id}", UpdateShip)
      .RequireAdmin();

    return app;
  }

  private static RouteGroupBuilder MapEndpoints<TInternal, T>(this RouteGroupBuilder builder)
  where TInternal : InternalEarnable
  where T : Earnable
  {
    builder.MapGet(string.Empty, GetEarnables<TInternal, T>)
      .AllowAnonymous();

    builder.MapGet("/{id}",  GetEarnable<TInternal, T>)
      .AllowAnonymous();

    builder.MapGet("/name/{name}",  GetEarnableByName<TInternal, T>)
      .AllowAnonymous();

    return builder;
  }

  public static async Task<Results<Ok<IEnumerable<T>>, ProblemHttpResult>> GetEarnables<TInternal, T>(
    IEarnableRepository<TInternal> earnableRepository,
    IMapper<TInternal, T> mapper)
  where TInternal : InternalEarnable
  where T : Earnable
  {
    var earnables =  await earnableRepository.GetEarnables();
    return TypedResults.Ok(earnables.Select(mapper.MapTo));
  }

  public static async Task<Results<Ok<T>, ProblemHttpResult>> GetEarnable<TInternal, T>(string id,
    IEarnableRepository<TInternal> earnableRepository,
    IMapper<TInternal, T> mapper)
    where TInternal : InternalEarnable
    where T : Earnable
  {
    var earnable = await earnableRepository.GetEarnable(id);
    if (earnable is null)
    {
      return TypedResults.Problem(detail:"No entity with that ID exists.",
        statusCode:StatusCodes.Status404NotFound);
    }

    return TypedResults.Ok(mapper.MapTo(earnable));
  }

  public static async Task<Results<Ok<T>, ProblemHttpResult>> GetEarnableByName<TInternal, T>(string name,
    IEarnableRepository<TInternal> earnableRepository,
    IMapper<TInternal, T> mapper)
    where TInternal : InternalEarnable
    where T : Earnable
  {
    var earnable = await earnableRepository.GetEarnableByName(name);
    if (earnable is null)
    {
      return TypedResults.Problem(detail:"No entity with that name exists.",
        statusCode:StatusCodes.Status404NotFound);
    }

    return TypedResults.Ok(mapper.MapTo(earnable));
  }

  public static async Task<Results<Ok<Character>, ProblemHttpResult>> CreateCharacter(
    CreateCharacterRequest request,
    ICharacterRepository characterRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalCharacter, Character> characterMapper)
  {
    var existingCharacter = await characterRepository.GetEarnableByName(request.Name);
    if (existingCharacter is not null)
    {
      return TypedResults.Problem("A character with that name already exists.",
        statusCode:StatusCodes.Status400BadRequest);
    }

    var locations = request.Locations.Select(earnableLocationMapper.MapFrom)
      .ToList();

    var character = await characterRepository.CreateCharacter(request.Name,
      locations,
      request.IsAccelerated);

    if (request.Marquee is not null)
    {
      var marquee = await marqueeRepository.CreateMarquee(character,
        request.Marquee.IntroductionDate,
        request.Marquee.MarqueeEventDate,
        request.Marquee.ShipmentDate,
        request.Marquee.FarmDate,
        request.Marquee.AccelerationDate);

      character.Marquee = marquee;
    }

    return TypedResults.Ok(characterMapper.MapTo(character));
  }

  public static async Task<Results<Ok<Ship>, ProblemHttpResult>> CreateShip(
    CreateShipRequest request,
    IShipRepository shipRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalShip, Ship> shipMapper)
  {
    var existingShip = await shipRepository.GetEarnableByName(request.Name);
    if (existingShip is not null)
    {
      return TypedResults.Problem("A Ship with that name already exists.",
        statusCode:StatusCodes.Status400BadRequest);
    }

    var locations = request.Locations.Select(earnableLocationMapper.MapFrom)
      .ToList();

    var ship = await shipRepository.CreateShip(request.Name,
      locations);

    if (request.Marquee is not null)
    {
      var marquee = await marqueeRepository.CreateMarquee(ship,
        request.Marquee.IntroductionDate,
        request.Marquee.MarqueeEventDate,
        request.Marquee.ShipmentDate,
        request.Marquee.FarmDate,
        null);

      ship.Marquee = marquee;
    }

    return TypedResults.Ok(shipMapper.MapTo(ship));
  }

  public static async Task<Results<Ok<Character>, ProblemHttpResult>> UpdateCharacter(string id,
    UpdateCharacterRequest request,
    IEarnableRepository<InternalCharacter> characterRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalCharacter, Character> characterMapper,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper)
  {
    return await UpdateEarnable(id,
      request.Locations,
      request.Marquee,
      request.Marquee?.AccelerationDate,
      characterRepository,
      marqueeRepository,
      characterMapper,
      earnableLocationMapper,
      internalCharacter =>
      {
        if (request.IsAccelerated is not null)
        {
          internalCharacter.IsAccelerated = request.IsAccelerated.Value;
        }
      });
  }

  public static async Task<Results<Ok<Ship>, ProblemHttpResult>> UpdateShip(string id,
    UpdateShipRequest request,
    IEarnableRepository<InternalShip> shipRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalShip, Ship> shipMapper,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper)
  {
    return await UpdateEarnable(id,
      request.Locations,
      request.Marquee,
      null,
      shipRepository,
      marqueeRepository,
      shipMapper,
      earnableLocationMapper);
  }

  private static async Task<Results<Ok<T>, ProblemHttpResult>> UpdateEarnable<TInternal, T>(string id,
    EarnableLocation[]? locations,
    MarqueeRequest? marquee,
    DateOnly? marqueeAccelerationDate,
    IEarnableRepository<TInternal> earnableRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<TInternal, T> mapper,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    Action<TInternal>? configureEarnable = null)
  where TInternal : InternalEarnable
  where T : Earnable
  {
    var internalEarnable = await earnableRepository.GetEarnable(id);
    if (internalEarnable is null)
    {
      return TypedResults.Problem(detail: "No entity with that ID exist.",
        statusCode: StatusCodes.Status404NotFound);
    }

    if (locations is not null)
    {
      internalEarnable.Locations = locations.Select(earnableLocationMapper.MapFrom)
        .ToList();
    }

    configureEarnable?.Invoke(internalEarnable);

    await earnableRepository.SaveEarnable(internalEarnable);

    if (marquee is not null)
    {
      if (internalEarnable.Marquee is not null)
      {
        internalEarnable.Marquee.IntroductionDate = marquee.IntroductionDate;
        internalEarnable.Marquee.MarqueeEventDate = marquee.MarqueeEventDate;
        internalEarnable.Marquee.ShipmentDate = marquee.ShipmentDate;
        internalEarnable.Marquee.FarmDate = marquee.FarmDate;
        internalEarnable.Marquee.AccelerationDate = marqueeAccelerationDate;

        await marqueeRepository.SaveMarquee(internalEarnable.Marquee);
      }
      else
      {
        internalEarnable.Marquee = await marqueeRepository.CreateMarquee(
          internalEarnable,
          marquee.IntroductionDate,
          marquee.MarqueeEventDate,
          marquee.ShipmentDate,
          marquee.FarmDate,
          marqueeAccelerationDate);
      }
    }

    return TypedResults.Ok(mapper.MapTo(internalEarnable));
  }
}
