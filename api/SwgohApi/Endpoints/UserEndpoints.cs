using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SwgohApi.Configuration;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Mapping;
using SwgohApi.Models.Users;
using SwgohApi.Services;

namespace SwgohApi.Endpoints;

public static class UserEndpoints
{
  public static WebApplication MapUserEndpoints(this WebApplication app)
  {
    var users = app.MapGroup("/api/users")
      .RequireAuthorization();

    users.MapGet(string.Empty, GetUsers);
    users.MapPost(string.Empty, CreateUser)
      .AllowAnonymous();
    users.MapPut("/{id}", UpdateUser);
    users.MapPut("/{id}/updateAdmin", UpdateAdmin)
      .RequireAdmin();
    users.MapDelete("/{id}", DeleteUser);

    return app;
  }

  public static async Task<Results<Ok<IEnumerable<UserDto>>, ProblemHttpResult>> GetUsers(
    IUserRepository userRepository,
    IMapper<User, UserDto> userMapper)
  {
    var users = await userRepository.GetUsers();
    var usersResponse = users.Select(userMapper.MapTo);

    return TypedResults.Ok(usersResponse);
  }

  public static async Task<Results<Ok<UserDto>, ProblemHttpResult>> CreateUser(
    CreateUserRequest request,
    IUserRepository userRepository,
    IMapper<User, UserDto> userMapper,
    IOptions<UserEndpointsConfiguration> config)
  {
    if (config.Value.CreateUsersKey != request.Key)
    {
      return TypedResults.Problem(statusCode: (int)HttpStatusCode.Forbidden);
    }

    var existingUser = await userRepository.GetUserByEmail(request.Email);
    if (existingUser is not null)
    {
      return TypedResults.Problem("A user with that email address already exists.",
        statusCode:(int)HttpStatusCode.BadRequest);
    }

    var user = await userRepository.CreateUser(request.Email, request.Password);
    return TypedResults.Ok(userMapper.MapTo(user));
  }

  public static async Task<Results<Ok<UserDto>, ProblemHttpResult>> UpdateUser(
    [FromRoute(Name = "id")] string userId,
    UpdateUserRequest request,
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    IMapper<User, UserDto> userMapper,
    HttpContext httpContext)
  {
    var requestingUser = httpContext.RequestingUser;
    if (requestingUser is null)
    {
      return TypedResults.Problem(statusCode: (int)HttpStatusCode.Forbidden);
    }

    // Only Admins or the actual user can update the user.
    User? user;
    if (requestingUser.Id == userId)
    {
      user = requestingUser;
    }
    else
    {
      if (!requestingUser.IsAdmin)
      {
        return TypedResults.Problem(statusCode: (int)HttpStatusCode.Forbidden);
      }

      user = await userRepository.GetUserById(userId);
      if (user is null)
      {
        return TypedResults.Problem("A user with that ID does not exist",
          statusCode:(int)HttpStatusCode.NotFound);
      }
    }

    if (string.IsNullOrEmpty(request.Email) &&
        string.IsNullOrEmpty(request.Password))
    {
      return TypedResults.Ok(userMapper.MapTo(user));
    }

    if (!string.IsNullOrEmpty(request.Email))
    {
      user.Email = request.Email;
    }

    if (!string.IsNullOrEmpty(request.Password))
    {
      user.Password = passwordHasher.HashPassword(user, request.Password!);
    }

    await userRepository.SaveUser(user);
    return TypedResults.Ok(userMapper.MapTo(user));
  }

  public static async Task<Results<Ok, ProblemHttpResult>> DeleteUser(
    [FromRoute(Name = "id")] string userId,
    IUserRepository userRepository,
    IAuthService authService,
    HttpContext httpContext)
  {
    var requestingUser = httpContext.RequestingUser;
    if (requestingUser is null)
    {
      return TypedResults.Problem(statusCode: (int)HttpStatusCode.Forbidden);
    }

    // Only Admins or the actual user can delete the user.
    if (!(requestingUser.Id == userId ||
        requestingUser.IsAdmin))
    {
      return TypedResults.Problem(statusCode: (int)HttpStatusCode.Forbidden);
    }

    // Revoke all the user's refresh tokens
    await authService.RevokeAll(userId);

    var result = await userRepository.DeleteUser(userId);
    if (!result)
    {
      return TypedResults.Problem("A user with that ID does not exist",
        statusCode:(int)HttpStatusCode.NotFound);
    }

    return TypedResults.Ok();
  }

  public static async Task<Results<Ok<UserDto>, ProblemHttpResult>> UpdateAdmin(
    [FromRoute(Name = "id")] string userId,
    UpdateAdminRequest request,
    IUserRepository userRepository,
    IMapper<User, UserDto> userMapper)
  {
    var user =  await userRepository.GetUserById(userId);
    if (user is null)
    {
      return TypedResults.Problem("A user with that ID does not exist",
        statusCode:(int)HttpStatusCode.NotFound);
    }

    user.IsAdmin = request.IsAdmin;
    await userRepository.SaveUser(user);
    return TypedResults.Ok(userMapper.MapTo(user));
  }
}
