using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Endpoints;

public static class ShipEndpoints
{
  public static WebApplication MapShipEndpoints(this WebApplication app)
  {
    var ships = app.MapGroup("/ships")
      .RequireAuthorization();

    ships.MapPost(string.Empty, CreateShip)
      .RequireAdmin();

    ships.MapGet("/{id}",  GetShip)
      .AllowAnonymous();
    ships.MapGet("/name/{name}", GetShipByName)
      .AllowAnonymous();

    ships.MapPut("/{id}", UpdateShip)
      .RequireAdmin();

    return app;
  }

  public static async Task<Results<Ok<Ship>, ProblemHttpResult>> CreateShip(
    CreateShipRequest request,
    IShipRepository shipRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper,
    IMapper<InternalShip, Ship> shipMapper)
  {
    var existingShip = await shipRepository.GetShipByName(request.Name);
    if (existingShip is not null)
    {
      return TypedResults.Problem("A Ship with that name already exists.",
        statusCode:(int)HttpStatusCode.BadRequest);
    }

    var locations = request.Locations.Select(earnableLocationMapper.MapFrom)
      .ToList();

    var ship = await shipRepository.CreateShip(request.Name,
      locations,
      null);

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

  public static async Task<Results<Ok<IEnumerable<Ship>>, ProblemHttpResult>> GetShips(
    IShipRepository shipRepository,
    IMapper<InternalShip, Ship> shipMapper)
  {
    var ships = await shipRepository.GetShips();

    return TypedResults.Ok(ships.Select(shipMapper.MapTo));
  }

  public static async Task<Results<Ok<Ship>, ProblemHttpResult>> GetShip(string id,
    IShipRepository shipRepository,
    IMapper<InternalShip, Ship> shipMapper)
  {
    var ship = await shipRepository.GetShip(id);
    if (ship is null)
    {
      return TypedResults.Problem(detail:"No ship with that ID exists.",
        statusCode:(int)HttpStatusCode.NotFound);
    }

    return TypedResults.Ok(shipMapper.MapTo(ship));
  }

  public static async Task<Results<Ok<Ship>, ProblemHttpResult>> UpdateShip(string id,
    UpdateShipRequest request,
    IShipRepository shipRepository,
    IMarqueeRepository marqueeRepository,
    IMapper<InternalShip, Ship> shipMapper,
    IMapper<InternalEarnableLocation, EarnableLocation> earnableLocationMapper)
  {
    var internalShip = await shipRepository.GetShip(id);
    if (internalShip is null)
    {
      return TypedResults.Problem(detail:"No ship with that ID exists.",
        statusCode:(int)HttpStatusCode.NotFound);
    }

    if (request.Locations is not null)
    {
      internalShip.Locations = request.Locations.Select(earnableLocationMapper.MapFrom)
        .ToList();
    }

    await shipRepository.SaveShip(internalShip);

    if (request.Marquee is not null)
    {
      if (internalShip.Marquee is not null)
      {
        internalShip.Marquee.IntroductionDate = request.Marquee.IntroductionDate;
        internalShip.Marquee.MarqueeEventDate = request.Marquee.MarqueeEventDate;
        internalShip.Marquee.ShipmentDate = request.Marquee.ShipmentDate;
        internalShip.Marquee.FarmDate = request.Marquee.FarmDate;
        internalShip.Marquee.AccelerationDate = null;

        await marqueeRepository.SaveMarquee(internalShip.Marquee);
      }
      else
      {
        internalShip.Marquee = await marqueeRepository.CreateMarquee(
          internalShip,
          request.Marquee.IntroductionDate,
          request.Marquee.MarqueeEventDate,
          request.Marquee.ShipmentDate,
          request.Marquee.FarmDate,
          null);
      }
    }

    return TypedResults.Ok(shipMapper.MapTo(internalShip));
  }

  public static async Task<Results<Ok<Ship>, ProblemHttpResult>> GetShipByName(string name,
    IShipRepository shipRepository,
    IMapper<InternalShip, Ship> shipMapper)
  {
    var character = await shipRepository.GetShipByName(name);
    if (character is null)
    {
      return TypedResults.Problem(detail:"No ship with that name exists.",
        statusCode:(int)HttpStatusCode.NotFound);
    }

    return TypedResults.Ok(shipMapper.MapTo(character));
  }
}
