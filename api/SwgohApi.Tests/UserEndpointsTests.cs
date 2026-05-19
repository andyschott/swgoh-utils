using AutoFixture.Xunit3;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Users;

namespace SwgohApi.Tests;

public sealed class UserEndpointsTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IUserRepository> _mockUserRepository;

  public UserEndpointsTests()
  {
    _mockUserRepository = _mockRepository.Create<IUserRepository>();
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
    _mockUserRepository.Setup(repository => repository.CreateUser(request.Email, request.Password))
      .ReturnsAsync(user);

    var response = await UserEndpoints.CreateUser(request, _mockUserRepository.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.NotNull(okResult.Value);
    Assert.Equal(user.Id, okResult.Value.Id);
    Assert.Equal(user.Email, okResult.Value.Email);
  }

}
