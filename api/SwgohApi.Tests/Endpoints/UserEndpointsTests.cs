using System.Net;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SwgohApi.Configuration;
using SwgohApi.Endpoints;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Mapping;
using SwgohApi.Models.Users;
using SwgohApi.Services;

namespace SwgohApi.Tests.Endpoints;

public sealed class UserEndpointsTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IUserRepository> _mockUserRepository;
  private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
  private readonly Mock<IMapper<User, UserDto>> _mockUserMapper;
  private readonly Mock<IAuthService> _mockAuthService;

  private readonly HttpContext _httpContext = new DefaultHttpContext();

  public UserEndpointsTests()
  {
    _mockUserRepository = _mockRepository.Create<IUserRepository>();
    _mockPasswordHasher = _mockRepository.Create<IPasswordHasher<User>>();
    _mockUserMapper = _mockRepository.Create<IMapper<User, UserDto>>();
    _mockAuthService = _mockRepository.Create<IAuthService>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, AutoData]
  public async Task GetUsers_Successful(User[] users,
    UserDto[] responseUsers)
  {
    _mockUserRepository.Setup(repository => repository.GetUsers())
      .ReturnsAsync(users);

    foreach (var (src, dest) in users.Zip(responseUsers))
    {
      _mockUserMapper.Setup(mapper => mapper.MapTo(src))
        .Returns(dest);
    }

    var response = await UserEndpoints.GetUsers(_mockUserRepository.Object,
      _mockUserMapper.Object);

    var result = Assert.IsType<Results<Ok<IEnumerable<UserDto>>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<IEnumerable<UserDto>>>(result.Result);
    Assert.NotNull(okResult.Value);

    Assert.Equal(responseUsers.Length, okResult.Value.Count());
    Assert.All(responseUsers, user =>
    {
      Assert.Contains(okResult.Value, userDto => userDto.Id == user.Id);
    });
  }

  [Theory, AutoData]
  public async Task CreateUser_Successful(CreateUserRequest request, User user,
    UserDto responseUser)
  {
    var mockConfig = _mockRepository.Create<IOptions<UserEndpointsConfiguration>>();
    mockConfig.Setup(options => options.Value)
      .Returns(new UserEndpointsConfiguration
      {
        CreateUsersKey = request.Key
      });

    _mockUserRepository.Setup(repository => repository.GetUserByEmail(request.Email))
      .ReturnsAsync((User?)null);
    _mockUserRepository.Setup(repository => repository.CreateUser(request.Email, request.Password))
      .ReturnsAsync(user);
    _mockUserMapper.Setup(mapper => mapper.MapTo(user))
      .Returns(responseUser);
    var response = await UserEndpoints.CreateUser(request,
      _mockUserRepository.Object,
      _mockUserMapper.Object,
      mockConfig.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.Same(responseUser, okResult.Value);
  }

  [Theory, AutoData]
  public async Task CreateUser_InvalidKey_ReturnsForbidden(CreateUserRequest request,
    string key)
  {
    var mockConfig = _mockRepository.Create<IOptions<UserEndpointsConfiguration>>();
    mockConfig.Setup(options => options.Value)
      .Returns(new UserEndpointsConfiguration
      {
        CreateUsersKey = key
      });

    var response = await UserEndpoints.CreateUser(request,
      _mockUserRepository.Object,
      _mockUserMapper.Object,
      mockConfig.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal((int)HttpStatusCode.Forbidden, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task CreateUser_MissingKey_ReturnsForbidden(string key,
    IFixture fixture)
  {
    var mockConfig = _mockRepository.Create<IOptions<UserEndpointsConfiguration>>();
    mockConfig.Setup(options => options.Value)
      .Returns(new UserEndpointsConfiguration
      {
        CreateUsersKey = key
      });

    var request = fixture.Build<CreateUserRequest>()
      .With(request => request.Key, (string?)null)
      .Create();
    var response = await UserEndpoints.CreateUser(request,
      _mockUserRepository.Object,
      _mockUserMapper.Object,
      mockConfig.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal((int)HttpStatusCode.Forbidden, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task CreateUser_UserAlreadyExists_ReturnsBadRequest(CreateUserRequest request,
    User user)
  {
    var mockConfig = _mockRepository.Create<IOptions<UserEndpointsConfiguration>>();
    mockConfig.Setup(options => options.Value)
      .Returns(new UserEndpointsConfiguration
      {
        CreateUsersKey = request.Key
      });
    _mockUserRepository.Setup(repository => repository.GetUserByEmail(request.Email))
      .ReturnsAsync(user);

    var response = await UserEndpoints.CreateUser(request,
      _mockUserRepository.Object,
      _mockUserMapper.Object,
      mockConfig.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.BadRequest, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateUser_UserDoesNotExist_ReturnsNotFound(string userId,
    UpdateUserRequest request,
    IFixture fixture)
  {
    var requestingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, true)
      .Create();
    _httpContext.RequestingUser = requestingUser;

    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync((User?)null);

    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateUser_NoUpdatesProvided_ReturnsUserWithoutSaving(string userId,
    UserDto responseUser,
    IFixture fixture)
  {
    var user = fixture.Build<User>()
      .With(user => user.Id, userId)
      .Create();
    _httpContext.RequestingUser = user;
    _mockUserMapper.Setup(mapper => mapper.MapTo(user))
      .Returns(responseUser);

    var request = new UpdateUserRequest(null, null);

    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.Same(responseUser, okResult.Value);
  }

  [Theory, AutoData]
  public async Task UpdateUser_EmailAndPasswordProvided_UpdatesAndSavesUser(string userId,
    string email,
    string password,
    string hashedPassword,
    UserDto responseUser,
    IFixture fixture)
  {
    var user = fixture.Build<User>()
      .With(user => user.Id, userId)
      .Create();
    _httpContext.RequestingUser = user;
    _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, password))
      .Returns(hashedPassword);
    _mockUserRepository.Setup(repository => repository.SaveUser(user))
      .Returns(Task.CompletedTask);
    _mockUserMapper.Setup(mapper => mapper.MapTo(user))
      .Returns(responseUser);

    var request = new UpdateUserRequest(email, password);

    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.Same(responseUser, okResult.Value);
  }

  [Theory, AutoData]
  public async Task UpdateUser_RequestingUserIsNotAdmin_ReturnsForbidden(string userId,
    UpdateUserRequest request,
    IFixture fixture)
  {
    var requestingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, false)
      .Create();
    _httpContext.RequestingUser = requestingUser;

    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal((int)HttpStatusCode.Forbidden, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateUser_RequestingUserNotFound_ReturnsForbidden(string userId,
    UpdateUserRequest request)
  {
    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal((int)HttpStatusCode.Forbidden, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task DeleteUser_RequestingUserIsNotAdmin_ReturnsForbidden(string userId,
    UpdateUserRequest request,
    IFixture fixture)
  {
    var requestingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, false)
      .Create();
    _httpContext.RequestingUser = requestingUser;

    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal((int)HttpStatusCode.Forbidden, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task DeleteUser_Successful(string userId,
    IFixture fixture)
  {
    var requestingUser = fixture.Build<User>()
      .With(user => user.Id, userId)
      .Create();
    _httpContext.RequestingUser = requestingUser;

    _mockUserRepository.Setup(repository => repository.DeleteUser(userId))
      .ReturnsAsync(true);

    _mockAuthService.Setup(service => service.RevokeAll(userId))
      .Returns(Task.CompletedTask);

    var response = await UserEndpoints.DeleteUser(userId,
      _mockUserRepository.Object,
      _mockAuthService.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok, ProblemHttpResult>>(response);
    Assert.IsType<Ok>(result.Result);
  }

  [Theory, AutoData]
  public async Task DeleteUser_UserDoesNotExist_ReturnsNotFound(string userId,
    IFixture fixture)
  {
    var requestingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, true)
      .Create();
    _httpContext.RequestingUser = requestingUser;

    _mockUserRepository.Setup(repository => repository.DeleteUser(userId))
      .ReturnsAsync(false);

    _mockAuthService.Setup(service => service.RevokeAll(userId))
      .Returns(Task.CompletedTask);

    var response = await UserEndpoints.DeleteUser(userId,
      _mockUserRepository.Object,
      _mockAuthService.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateAdmin_Successful(string userId,
    UserDto responseUser,
    IFixture fixture)
  {
    var updatingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, false)
      .Create();
    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync(updatingUser);

    _mockUserRepository.Setup(repository => repository.SaveUser(
        It.Is<User>(user => user.Id == updatingUser.Id && updatingUser.IsAdmin)))
      .Returns(Task.CompletedTask);

    _mockUserMapper.Setup(mapper => mapper.MapTo(updatingUser))
      .Returns(responseUser);

    var response = await UserEndpoints.UpdateAdmin(userId,
      new UpdateAdminRequest(true),
      _mockUserRepository.Object,
      _mockUserMapper.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.Same(responseUser, okResult.Value);
  }

  [Theory, AutoData]
  public async Task UpdateAdmin_UserDoesNotExist_ReturnsNotFound(string userId)
  {
    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync((User?)null);

    var response = await UserEndpoints.UpdateAdmin(userId,
      new UpdateAdminRequest(true),
      _mockUserRepository.Object,
      _mockUserMapper.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }
}
