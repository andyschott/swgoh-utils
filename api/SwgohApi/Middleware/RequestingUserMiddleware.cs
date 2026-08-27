using System.Security.Claims;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Services;

namespace SwgohApi.Middleware;

public class RequestingUserMiddleware
{
  private readonly RequestDelegate _next;

  public RequestingUserMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task Invoke(HttpContext context,
    ITokenService tokenService,
    IUserRepository userRepository)
  {
    var claims = await tokenService.GetClaims(context);
    var userId = claims?.GetValueOrDefault(ClaimTypes.NameIdentifier);
    if (userId is null)
    {
      await _next(context);
      return;
    }

    var requestingUser = await userRepository.GetUserById(userId.Value);
    if (requestingUser is null)
    {
      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
      return;
    }

    context.RequestingUser = requestingUser;
    await _next(context);
  }
}
