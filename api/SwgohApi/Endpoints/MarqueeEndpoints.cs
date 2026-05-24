using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Endpoints;

public static class MarqueeEndpoints
{
  public static WebApplication MapMarqueeEndpoints(this WebApplication app)
  {
    var marquees = app.MapGroup("/marquees");

    marquees.MapGet(string.Empty, GetMarquees)
      .AllowAnonymous();

    return app;
  }

  public static async Task<Ok<IEnumerable<MarqueeDate>>> GetMarquees(
    IMarqueeRepository marqueeRepository,
    IMapper<InternalMarquee, MarqueeDate> marqueeDateMapper)
  {
    var marquees = await marqueeRepository.GetMarquees();

    return TypedResults.Ok(marquees.Select(marqueeDateMapper.MapTo));
  }
}
