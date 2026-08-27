using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Models.Auth;
using SwgohApi.Services;

namespace SwgohApi.Endpoints;

public static class AuthEndpoints
{
  public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder builder)
  {
    var auth = builder.MapGroup("/auth")
      .AllowAnonymous();

    auth.MapPost("/login", Login);
    auth.MapPost("/refresh", Refresh);
    auth.MapPost("/revoke", Revoke);
    auth.MapPost("/revoke-all", RevokeAll);

    return builder;
  }

  public static async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> Login(
    LoginRequest request,
    IAuthService authService)
  {
    var tokenResponse = await authService.Login(request);
    if (tokenResponse is null)
    {
      return TypedResults.Unauthorized();
    }

    return TypedResults.Ok(tokenResponse);
  }

  public static async Task<Results<Ok<TokenResponse>, UnauthorizedHttpResult>> Refresh(
    RefreshRequest request,
    IAuthService authService)
  {
    var tokenResponse = await authService.Refresh(request);
    if (tokenResponse is null)
    {
      return TypedResults.Unauthorized();
    }

    return TypedResults.Ok(tokenResponse);
  }

  public static async Task<Ok> Revoke(
    RevokeRequest request,
    IAuthService authService)
  {
    await authService.Revoke(request);
    return TypedResults.Ok();
  }

  public static async Task<Results<Ok, UnauthorizedHttpResult>> RevokeAll(
    HttpContext context,
    IAuthService authService)
  {
    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
    {
      return TypedResults.Unauthorized();
    }

    await authService.RevokeAll(userId);
    return TypedResults.Ok();
  }
}
