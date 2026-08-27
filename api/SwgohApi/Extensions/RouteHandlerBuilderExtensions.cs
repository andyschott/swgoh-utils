using SwgohApi.Services;

namespace SwgohApi.Extensions;

public static class RouteHandlerBuilderExtensions
{
  extension(RouteHandlerBuilder builder)
  {
    public RouteHandlerBuilder RequireAdmin()
    {
      return builder.RequireAuthorization(Policies.ApiJwtAdmin);
    }
  }
}
