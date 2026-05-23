using SwgohApi.Filters;

namespace SwgohApi.Extensions;

public static class RouteHandlerBuilderExtensions
{
  extension(RouteHandlerBuilder builder)
  {
    public RouteHandlerBuilder RequireAdmin()
    {
      return builder.AddEndpointFilter<RequireAdminEndpointFilter>();
    }
  }
}
