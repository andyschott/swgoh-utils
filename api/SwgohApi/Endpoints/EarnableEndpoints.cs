using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalConquestRewardPhase = SwgohApi.Infrastructure.Models.ConquestRewardPhase;

namespace SwgohApi.Endpoints;

public static class EarnableEndpoints
{
  public static WebApplication MapEarnableEndpoints(this WebApplication app)
  {
    var characters = app.MapGroup("/api/characters")
      .RequireAuthorization()
      .MapEndpoints<InternalCharacter, Character>();
    characters.MapPost(string.Empty, CreateCharacter)
      .RequireAdmin();
    characters.MapPut("/{id}", UpdateCharacter)
      .RequireAdmin();
    characters.MapPost("/import", ImportCharacters)
      .RequireAdmin();

    var ships = app.MapGroup("/api/ships")
      .RequireAuthorization()
      .MapEndpoints<InternalShip, Ship>();
    ships.MapPost(string.Empty, CreateShip)
      .RequireAdmin();
    ships.MapPut("/{id}", UpdateShip)
      .RequireAdmin();
    ships.MapPost("/import", ImportShips)
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
    var character = await CreateCharacterImpl(request, characterRepository, marqueeRepository, earnableLocationMapper);
    if (character is null)
    {
      return TypedResults.Problem("A character with that name already exists.",
        statusCode:StatusCodes.Status400BadRequest);
    }

    return TypedResults.Ok(characterMapper.MapTo(character));
  }

  public static async Task<Ok<IEnumerable<ImportResult>>> ImportCharacters(
    CreateCharacterRequest[] requests,
    ICharacterRepository characterRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalCharacter, Character> characterMapper)
  {
    var results = new List<ImportResult>();

    foreach (var request in requests)
    {
      var character = await CreateCharacterImpl(request, characterRepository, marqueeRepository, earnableLocationMapper);
      if (character is null)
      {
        results.Add(new ImportResult(request.Name, false));
      }
      else
      {
        results.Add(new ImportResult(request.Name, true));
      }
    }

    return TypedResults.Ok(results.AsEnumerable());
  }

  private static async Task<InternalCharacter?> CreateCharacterImpl(CreateCharacterRequest request,
    ICharacterRepository characterRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper)
  {
    var existingCharacter = await characterRepository.GetEarnableByName(request.Name);
    if (existingCharacter is not null)
    {
      return null;
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

    return character;
  }

  public static async Task<Results<Ok<Ship>, ProblemHttpResult>> CreateShip(
    CreateShipRequest request,
    IShipRepository shipRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalShip, Ship> shipMapper)
  {
    var ship = await CreateShipImpl(request, shipRepository, marqueeRepository, earnableLocationMapper);
    if (ship is null)
    {
      return TypedResults.Problem("A Ship with that name already exists.",
        statusCode:StatusCodes.Status400BadRequest);
    }

    return TypedResults.Ok(shipMapper.MapTo(ship));
  }

  private static async Task<InternalShip?> CreateShipImpl(CreateShipRequest request,
    IShipRepository shipRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper)
  {
    var existingShip = await shipRepository.GetEarnableByName(request.Name);
    if (existingShip is not null)
    {
      return null;
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

    return ship;
  }

  public static async Task<Ok<IEnumerable<ImportResult>>> ImportShips(
    CreateShipRequest[] requests,
    IShipRepository shipRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalShip, Ship> shipMapper)
  {
    var results = new List<ImportResult>();

    foreach (var request in requests)
    {
      var character = await CreateShipImpl(request, shipRepository, marqueeRepository, earnableLocationMapper);
      if (character is null)
      {
        results.Add(new ImportResult(request.Name, false));
      }
      else
      {
        results.Add(new ImportResult(request.Name, true));
      }
    }

    return TypedResults.Ok(results.AsEnumerable());
  }

  public static async Task<Results<Ok<Character>, ProblemHttpResult>> UpdateCharacter(string id,
    UpdateCharacterRequest request,
    IEarnableRepository<InternalCharacter> characterRepository,
    IMarqueeRepository marqueeRepository,
    IConquestRewardRepository conquestRewardRepository,
    IMapper<InternalCharacter, Character> characterMapper,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalConquestRewardPhase, ConquestRewardPhase> conquestRewardPhaseMapper)
  {
    return await UpdateEarnable(id,
      request.Locations,
      request.Marquee,
      request.Marquee?.AccelerationDate,
      request.ConquestReward,
      characterRepository,
      marqueeRepository,
      conquestRewardRepository,
      characterMapper,
      earnableLocationMapper,
      conquestRewardPhaseMapper,
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
    IConquestRewardRepository conquestRewardRepository,
    IMapper<InternalShip, Ship> shipMapper,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalConquestRewardPhase, ConquestRewardPhase> conquestRewardPhaseMapper)
  {
    return await UpdateEarnable(id,
      request.Locations,
      request.Marquee,
      null,
      request.ConquestReward,
      shipRepository,
      marqueeRepository,
      conquestRewardRepository,
      shipMapper,
      earnableLocationMapper,
      conquestRewardPhaseMapper);
  }

  private static async Task<Results<Ok<T>, ProblemHttpResult>> UpdateEarnable<TInternal, T>(string id,
    EarnableLocation[]? locations,
    MarqueeRequest? marquee,
    DateOnly? marqueeAccelerationDate,
    ConquestRewardRequest? conquestReward,
    IEarnableRepository<TInternal> earnableRepository,
    IMarqueeRepository marqueeRepository,
    IConquestRewardRepository conquestRewardRepository,
    IMapper<TInternal, T> mapper,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalConquestRewardPhase, ConquestRewardPhase> conquestRewardPhaseMapper,
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

    if (conquestReward is not null)
    {
      var internalRewardPhase = conquestRewardPhaseMapper.MapFrom(conquestReward.RewardPhase);

      if (internalEarnable.ConquestReward is not null)
      {
        internalEarnable.ConquestReward.RewardPhase = internalRewardPhase;
        internalEarnable.ConquestReward.InitialUnlockDate = conquestReward.InitialUnlockDate;
        internalEarnable.ConquestReward.FinalRewardCreateDate = conquestReward.FinalRewardCreateDate;
        internalEarnable.ConquestReward.ProvingGroundsDate = conquestReward.ProvingGroundsDate;

        await conquestRewardRepository.SaveConquestReward(internalEarnable.ConquestReward);
      }
      else
      {
        internalEarnable.ConquestReward = await conquestRewardRepository.CreateConquestReward(
          internalEarnable,
          internalRewardPhase,
          conquestReward.InitialUnlockDate,
          conquestReward.FinalRewardCreateDate,
          conquestReward.ProvingGroundsDate);
      }
    }

    return TypedResults.Ok(mapper.MapTo(internalEarnable));
  }
}
