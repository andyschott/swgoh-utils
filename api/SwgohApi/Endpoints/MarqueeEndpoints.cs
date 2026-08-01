using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Endpoints;

public static class MarqueeEndpoints
{
  public static WebApplication MapMarqueeEndpoints(this WebApplication app)
  {
    var marquees = app.MapGroup("/api/marquees");

    marquees.MapGet(string.Empty, GetMarquees)
      .AllowAnonymous();
    marquees.MapPost("import", ImportMarquees)
      .RequireAdmin();

    return app;
  }

  public static async Task<Ok<IEnumerable<MarqueeDate>>> GetMarquees(
    IMarqueeRepository marqueeRepository,
    IMapper<InternalMarquee, MarqueeDate> marqueeDateMapper)
  {
    var marquees = await marqueeRepository.GetMarquees();

    return TypedResults.Ok(marquees.Select(marqueeDateMapper.MapTo));
  }

  public static async Task<Ok<IEnumerable<ImportResult>>> ImportMarquees(MarqueeDate[] marquees,
    ICharacterRepository characterRepository,
    IShipRepository shipRepository,
    IMarqueeRepository marqueeRepository)
  {
    var importedMarquees = new List<ImportResult>();

    foreach (var marquee in marquees)
    {
      InternalEarnable? earnable = await characterRepository.GetEarnableByName(marquee.Name);
      if (earnable is null)
      {
        earnable = await shipRepository.GetEarnableByName(marquee.Name);
        if (earnable is null)
        {
          importedMarquees.Add(new ImportResult(marquee.Name, false));
          continue;
        }
      }

      try
      {
        var importedMarquee = await marqueeRepository.CreateMarquee(earnable,
          marquee.IntroductionDate,
          marquee.MarqueeEventDate,
          marquee.ShipmentDate,
          marquee.FarmDate,
          marquee.AccelerationDate);
        importedMarquees.Add(new ImportResult(marquee.Name, true));
      }
      catch (Exception ex)
      {
        importedMarquees.Add(new ImportResult(marquee.Name, false));
      }
    }

    return TypedResults.Ok(importedMarquees.AsEnumerable());
  }
}
