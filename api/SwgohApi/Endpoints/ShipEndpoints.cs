using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Filters;
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
      .AddEndpointFilter<RequireAdminEndpointFilter>();

    ships.MapGet(string.Empty,  GetShips);

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
}
