using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Mapping;
using SwgohApi.Models.Users;
using SwgohApi.Services;

namespace SwgohApi.Endpoints;

public static class UserEndpoints
{
  public static WebApplication MapUserEndpoints(this WebApplication app,
    bool allowCreation)
  {
    var users = app.MapGroup("/users")
      .RequireAuthorization();

    users.MapGet(string.Empty, GetUsers);
    if (allowCreation)
    {
      users.MapPost(string.Empty, CreateUser)
        .AllowAnonymous();
    }
    users.MapPut("/{id}", UpdateUser);
    users.MapPut("/{id}/updateAdmin", UpdateAdmin);
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
    IMapper<User, UserDto> userMapper)
  {
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
    ITokenService tokenService,
    HttpContext httpContext)
  {
    var requestingUser = await GetUser(httpContext, tokenService, userRepository);
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

  public static async Task<Results<Ok<UserDto>, ProblemHttpResult>> UpdateAdmin(
    [FromRoute(Name = "id")] string userId,
    UpdateAdminRequest request,
    IUserRepository userRepository,
    IMapper<User, UserDto> userMapper,
    ITokenService tokenService,
    HttpContext httpContext)
  {
    var requestingUser = await GetUser(httpContext, tokenService, userRepository);
    if (requestingUser?.IsAdmin != true)
    {
      return TypedResults.Problem(statusCode: (int)HttpStatusCode.Forbidden);
    }

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

  private static async Task<User?> GetUser(HttpContext httpContext,
    ITokenService tokenService,
    IUserRepository userRepository)
  {
    var claims = await tokenService.GetClaims(httpContext);
    var userId = claims?.GetValueOrDefault(JwtRegisteredClaimNames.Sub);
    if (userId is null)
    {
      return null;
    }

    return await userRepository.GetUserById(userId.Value);
  }
}
