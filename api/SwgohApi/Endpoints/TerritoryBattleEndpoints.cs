using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using SwgohApi.Models.TerritoryBattles;

namespace SwgohApi.Endpoints;

public static class TerritoryBattleEndpoints
{
  public static RouteGroupBuilder MapTerritoryBattleEndpoints(this RouteGroupBuilder builder)
  {
    var tb = builder.MapGroup("/tb")
      .AllowAnonymous();

    tb.MapGet("rote/rewards", GetRoteRewards);

    return builder;
  }

  public static Ok<IEnumerable<RiseOfTheEmpireRewards>> GetRoteRewards(
    IOptions<RiseOfTheEmpire> rote)
  {
    IEnumerable<RiseOfTheEmpireRewards> rewards = rote.Value.Rewards;
    return TypedResults.Ok(rewards);
  }
}
