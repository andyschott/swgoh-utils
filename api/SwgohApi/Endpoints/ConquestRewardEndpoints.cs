using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;

namespace SwgohApi.Endpoints;

public static class ConquestRewardEndpoints
{
  public static WebApplication MapConquestRewardEndpoints(this WebApplication app)
  {
    var conquestRewards = app.MapGroup("/api/conquestRewards");

    conquestRewards.MapGet(string.Empty, GetConquestRewards)
      .AllowAnonymous();

    return app;
  }

  public static async Task<Ok<IEnumerable<ConquestRewardDate>>> GetConquestRewards(
    IConquestRewardRepository conquestRewardRepository,
    IMapper<InternalConquestReward, ConquestRewardDate> conquestRewardMapper)
  {
    var conquestRewards = await conquestRewardRepository.GetConquestRewards();

    return TypedResults.Ok(conquestRewards.Select(conquestRewardMapper.MapTo));
  }
}
