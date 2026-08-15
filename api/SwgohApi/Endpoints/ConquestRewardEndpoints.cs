namespace SwgohApi.Endpoints;

public static class ConquestRewardEndpoints
{
  public static WebApplication MapConquestRewardEndpoints(this WebApplication app)
  {
    var conquestRewards = app.MapGroup("/api/conquestRewards");

    return app;
  }
}
