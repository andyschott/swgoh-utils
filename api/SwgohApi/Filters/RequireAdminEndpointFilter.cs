using SwgohApi.Extensions;

namespace SwgohApi.Filters;

public class RequireAdminEndpointFilter : IEndpointFilter
{
  public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
    EndpointFilterDelegate next)
  {
    var requestingUser = context.HttpContext.RequestingUser;
    if (requestingUser?.IsAdmin != true)
    {
      return Results.Forbid();
    }

    return await next(context);
  }
}
