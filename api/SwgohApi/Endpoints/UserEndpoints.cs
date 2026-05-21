using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Models.Users;

namespace SwgohApi.Endpoints;

public static class UserEndpoints
{
  public static WebApplication MapUserEndpoints(this WebApplication app,
    bool allowCreatingUsers)
  {
    var users = app.MapGroup("/users")
      .RequireAuthorization();

    users.MapGet(string.Empty, GetUsers);
    if (allowCreatingUsers)
    {
      users.MapPost(string.Empty, CreateUser)
        .AllowAnonymous();
    }
    users.MapPut("/{id}", UpdateUser);
    users.MapDelete("/{id}", DeleteUser);

    return app;
  }

  public static async Task<Results<Ok<IEnumerable<UserDto>>, ProblemHttpResult>> GetUsers(
    IUserRepository userRepository)
  {
    var users = await userRepository.GetUsers();
    var usersResponse = users.Select(MapUser);

    return TypedResults.Ok(usersResponse);
  }

  public static async Task<Results<Ok<UserDto>, ProblemHttpResult>> CreateUser(
    CreateUserRequest request,
    IUserRepository userRepository)
  {
    var existingUser = await userRepository.GetUserByEmail(request.Email);
    if (existingUser is not null)
    {
      return TypedResults.Problem("A user with that email address already exists.",
        statusCode:(int)HttpStatusCode.BadRequest);
    }

    var user = await userRepository.CreateUser(request.Email, request.Password);
    return TypedResults.Ok(MapUser(user));
  }

  public static async Task<Results<Ok<UserDto>, ProblemHttpResult>> UpdateUser(
    [FromRoute(Name = "id")] string userId,
    UpdateUserRequest request,
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher)
  {
    var user =  await userRepository.GetUserById(userId);
    if (user is null)

    {
      return TypedResults.Problem("A user with that ID does not exist",
        statusCode:(int)HttpStatusCode.NotFound);
    }

    if (string.IsNullOrEmpty(request.Email) &&
        string.IsNullOrEmpty(request.Password))
    {
      return TypedResults.Ok(MapUser(user));
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
    return TypedResults.Ok(MapUser(user));
  }

  public static async Task<Results<Ok, ProblemHttpResult>> DeleteUser(
    [FromRoute(Name = "id")] string userId,
    IUserRepository userRepository)
  {
    var result = await userRepository.DeleteUser(userId);
    if (!result)
    {
      return TypedResults.Problem("A user with that ID does not exist",
        statusCode:(int)HttpStatusCode.NotFound);
    }

    return TypedResults.Ok();
  }

  private static UserDto MapUser(User user) => new(user.Id, user.Email);
}
