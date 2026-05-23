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

    ships.MapGet(string.Empty, GetShips)
      .AllowAnonymous();
    ships.MapGet("/{id}",  GetShip)
      .AllowAnonymous();

    ships.MapPut("/{id}", UpdateShip)
      .RequireAdmin();

    return app;
  }

  public static async Task<Results<Ok<Ship>, ProblemHttpResult>> CreateShip(
    CreateShipRequest request,
    IShipRepository shipRepository,
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
      locations);

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
    return TypedResults.Ok(shipMapper.MapTo(internalShip));
  }
}
