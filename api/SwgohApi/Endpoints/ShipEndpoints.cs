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

    ships.MapPut("/{id}", UpdateShip)
      .RequireAdmin();

    return app;
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
}
