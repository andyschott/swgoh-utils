using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Users;

public static class UserEndpoints
{
  public static WebApplication MapUserEndpoints(this WebApplication app)
  {
    app.MapGet("/users", GetUsers);
    // Disable for now until I get security figured out
    // app.MapPost("/users", CreateUser);

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
    var user = await userRepository.CreateUser(request.Email, request.Password);
    return TypedResults.Ok(MapUser(user));
  }

  private static UserDto MapUser(User user) => new(user.Id, user.Email);
}
