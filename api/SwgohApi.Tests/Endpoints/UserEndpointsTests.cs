using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using SwgohApi.Endpoints;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Models.Users;

namespace SwgohApi.Tests.Endpoints;

public sealed class UserEndpointsTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IUserRepository> _mockUserRepository;
  private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;

  public UserEndpointsTests()
  {
    _mockUserRepository = _mockRepository.Create<IUserRepository>();
    _mockPasswordHasher = _mockRepository.Create<IPasswordHasher<User>>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, AutoData]
  public async Task GetUsers_Successful(User[] users)
  {
    _mockUserRepository.Setup(repository => repository.GetUsers())
      .ReturnsAsync(users);

    var response = await UserEndpoints.GetUsers(_mockUserRepository.Object);

    var result = Assert.IsType<Results<Ok<IEnumerable<UserDto>>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<IEnumerable<UserDto>>>(result.Result);
    Assert.NotNull(okResult.Value);

    Assert.Equal(users.Length, okResult.Value.Count());
    Assert.All(users, user =>
    {
      Assert.Contains(okResult.Value, userDto => userDto.Id == user.Id);
    });
  }

  [Theory, AutoData]
  public async Task CreateUser_Successful(CreateUserRequest request, User user)
  {
    _mockUserRepository.Setup(repository => repository.GetUserByEmail(request.Email))
      .ReturnsAsync((User?)null);
    _mockUserRepository.Setup(repository => repository.CreateUser(request.Email, request.Password))
      .ReturnsAsync(user);

    var response = await UserEndpoints.CreateUser(request, _mockUserRepository.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Equal(user.Id, okResult.Value.Id);
    Assert.Equal(user.Email, okResult.Value.Email);
  }

  [Theory, AutoData]
  public async Task CreateUser_UserAlreadyExists_ReturnsBadRequest(CreateUserRequest request,
    User user)
  {
    _mockUserRepository.Setup(repository => repository.GetUserByEmail(request.Email))
      .ReturnsAsync(user);

    var response = await UserEndpoints.CreateUser(request, _mockUserRepository.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.BadRequest, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateUser_UserDoesNotExist_ReturnsNotFound(string userId, UpdateUserRequest request)
  {
    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync((User?)null);

    var response = await UserEndpoints.UpdateUser(userId, request, _mockUserRepository.Object,
      _mockPasswordHasher.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateUser_NoUpdatesProvided_ReturnsUserWithoutSaving(string userId, User user)
  {
    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync(user);

    var request = new UpdateUserRequest(null, null);

    var response = await UserEndpoints.UpdateUser(userId, request, _mockUserRepository.Object,
      _mockPasswordHasher.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Equal(user.Id, okResult.Value.Id);
    Assert.Equal(user.Email, okResult.Value.Email);
  }

  [Theory, AutoData]
  public async Task UpdateUser_EmailAndPasswordProvided_UpdatesAndSavesUser(string userId, User user,
    string email, string password, string hashedPassword)
  {
    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync(user);
    _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, password))
      .Returns(hashedPassword);
    _mockUserRepository.Setup(repository => repository.SaveUser(user))
      .Returns(Task.CompletedTask);

    var request = new UpdateUserRequest(email, password);

    var response = await UserEndpoints.UpdateUser(userId, request, _mockUserRepository.Object,
      _mockPasswordHasher.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Equal(user.Id, okResult.Value.Id);
    Assert.Equal(email, okResult.Value.Email);
    Assert.Equal(email, user.Email);
    Assert.Equal(hashedPassword, user.Password);
  }

  [Theory, AutoData]
  public async Task DeleteUser_Successful(string userId)
  {
    _mockUserRepository.Setup(repository => repository.DeleteUser(userId))
      .ReturnsAsync(true);

    var response = await UserEndpoints.DeleteUser(userId, _mockUserRepository.Object);

    var result = Assert.IsType<Results<Ok, ProblemHttpResult>>(response);
    Assert.IsType<Ok>(result.Result);
  }

  [Theory, AutoData]
  public async Task DeleteUser_UserDoesNotExist_ReturnsNotFound(string userId)
  {
    _mockUserRepository.Setup(repository => repository.DeleteUser(userId))
      .ReturnsAsync(false);

    var response = await UserEndpoints.DeleteUser(userId, _mockUserRepository.Object);

    var result = Assert.IsType<Results<Ok, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }
}
