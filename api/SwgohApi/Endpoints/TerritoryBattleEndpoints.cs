using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using SwgohApi.Models.TerritoryBattles;

namespace SwgohApi.Endpoints;

public static class TerritoryBattleEndpoints
{
  public static WebApplication MapTerritoryBattleEndpoints(this WebApplication app)
  {
    var tb = app.MapGroup("/api/tb")
      .AllowAnonymous();

    tb.MapGet("rote/rewards", GetRoteRewards);

    return app;
  }

  public static Ok<IEnumerable<RiseOfTheEmpireRewards>> GetRoteRewards(
    IOptions<RiseOfTheEmpire> rote)
  {
    IEnumerable<RiseOfTheEmpireRewards> rewards = rote.Value.Rewards;
    return TypedResults.Ok(rewards);
  }
}
