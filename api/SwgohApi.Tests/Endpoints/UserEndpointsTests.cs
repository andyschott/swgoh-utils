using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using SwgohApi.Endpoints;
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
  private readonly Mock<ITokenService> _mockTokenService;

  private readonly HttpContext _httpContext = new DefaultHttpContext();

  public UserEndpointsTests()
  {
    _mockUserRepository = _mockRepository.Create<IUserRepository>();
    _mockPasswordHasher = _mockRepository.Create<IPasswordHasher<User>>();
    _mockUserMapper = _mockRepository.Create<IMapper<User, UserDto>>();
    _mockTokenService = _mockRepository.Create<ITokenService>();
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
    _mockUserRepository.Setup(repository => repository.GetUserByEmail(request.Email))
      .ReturnsAsync((User?)null);
    _mockUserRepository.Setup(repository => repository.CreateUser(request.Email, request.Password))
      .ReturnsAsync(user);
    _mockUserMapper.Setup(mapper => mapper.MapTo(user))
      .Returns(responseUser);

    var response = await UserEndpoints.CreateUser(request,
      _mockUserRepository.Object,
      _mockUserMapper.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.Same(responseUser, okResult.Value);
  }

  [Theory, AutoData]
  public async Task CreateUser_UserAlreadyExists_ReturnsBadRequest(CreateUserRequest request,
    User user)
  {
    _mockUserRepository.Setup(repository => repository.GetUserByEmail(request.Email))
      .ReturnsAsync(user);

    var response = await UserEndpoints.CreateUser(request,
      _mockUserRepository.Object,
      _mockUserMapper.Object);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.BadRequest, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateUser_UserDoesNotExist_ReturnsNotFound(string userId,
    string requestingUserId,
    UpdateUserRequest request,
    IFixture fixture)
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>
      {
        [JwtRegisteredClaimNames.Sub] = new(JwtRegisteredClaimNames.Sub, requestingUserId),
      });

    var requestingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, true)
      .Create();
    _mockUserRepository.Setup(repository => repository.GetUserById(requestingUserId))
      .ReturnsAsync(requestingUser);

    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync((User?)null);

    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _mockTokenService.Object,
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
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>
      {
        [JwtRegisteredClaimNames.Sub] = new(JwtRegisteredClaimNames.Sub, userId),
      });

    var user = fixture.Build<User>()
      .With(user => user.Id, userId)
      .Create();
    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync(user);
    _mockUserMapper.Setup(mapper => mapper.MapTo(user))
      .Returns(responseUser);

    var request = new UpdateUserRequest(null, null);

    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _mockTokenService.Object,
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
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>
      {
        [JwtRegisteredClaimNames.Sub] = new(JwtRegisteredClaimNames.Sub, userId),
      });

    var user = fixture.Build<User>()
      .With(user => user.Id, userId)
      .Create();
    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync(user);
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
      _mockTokenService.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.Same(responseUser, okResult.Value);
  }

  [Theory, AutoData]
  public async Task UpdateUser_RequestingUserIsNotAdmin_ReturnsForbidden(string userId,
    UpdateUserRequest request,
    string requestingUserId,
    IFixture fixture)
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>
      {
        [JwtRegisteredClaimNames.Sub] = new(JwtRegisteredClaimNames.Sub, requestingUserId),
      });

    var requestingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, false)
      .Create();
    _mockUserRepository.Setup(repository => repository.GetUserById(requestingUserId))
      .ReturnsAsync(requestingUser);

    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _mockTokenService.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal((int)HttpStatusCode.Forbidden, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateUser_RequestingUserNotFound_ReturnsForbidden(string userId,
    UpdateUserRequest request,
    string requestingUserId)
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>
      {
        [JwtRegisteredClaimNames.Sub] = new(JwtRegisteredClaimNames.Sub, requestingUserId),
      });

    _mockUserRepository.Setup(repository => repository.GetUserById(requestingUserId))
      .ReturnsAsync((User?)null);

    var response = await UserEndpoints.UpdateUser(userId,
      request,
      _mockUserRepository.Object,
      _mockPasswordHasher.Object,
      _mockUserMapper.Object,
      _mockTokenService.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal((int)HttpStatusCode.Forbidden, problemResult.StatusCode);
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

  [Theory, AutoData]
  public async Task UpdateAdmin_Successful(string requestingUserId,
    string updatingUserId,
    UserDto responseUser,
    IFixture fixture)
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>
      {
        [JwtRegisteredClaimNames.Sub] = new(JwtRegisteredClaimNames.Sub, requestingUserId),
      });

    var requestingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, true)
      .Create();
    _mockUserRepository.Setup(repository => repository.GetUserById(requestingUserId))
      .ReturnsAsync(requestingUser);

    var updatingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, false)
      .Create();
    _mockUserRepository.Setup(repository => repository.GetUserById(updatingUserId))
      .ReturnsAsync(updatingUser);

    _mockUserRepository.Setup(repository => repository.SaveUser(
        It.Is<User>(user => user.Id == updatingUser.Id && updatingUser.IsAdmin)))
      .Returns(Task.CompletedTask);

    _mockUserMapper.Setup(mapper => mapper.MapTo(updatingUser))
      .Returns(responseUser);

    var response = await UserEndpoints.UpdateAdmin(updatingUserId,
      new UpdateAdminRequest(true),
      _mockUserRepository.Object,
      _mockUserMapper.Object,
      _mockTokenService.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var okResult = Assert.IsType<Ok<UserDto>>(result.Result);

    Assert.Same(responseUser, okResult.Value);
  }

  [Theory, AutoData]
  public async Task UpdateAdmin_UserDoesNotExist_ReturnsNotFound(string requestingUserId,
    string updatingUserId,
    IFixture fixture)
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>
      {
        [JwtRegisteredClaimNames.Sub] = new(JwtRegisteredClaimNames.Sub, requestingUserId),
      });

    var requestingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, true)
      .Create();
    _mockUserRepository.Setup(repository => repository.GetUserById(requestingUserId))
      .ReturnsAsync(requestingUser);

    _mockUserRepository.Setup(repository => repository.GetUserById(updatingUserId))
      .ReturnsAsync((User?)null);

    var response = await UserEndpoints.UpdateAdmin(updatingUserId,
      new UpdateAdminRequest(true),
      _mockUserRepository.Object,
      _mockUserMapper.Object,
      _mockTokenService.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.NotNull(problemResult.ProblemDetails.Detail);
    Assert.NotEmpty(problemResult.ProblemDetails.Detail);
    Assert.Equal((int)HttpStatusCode.NotFound, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateAdmin_RequestingUserIsNotAdmin_ReturnsForbidden(string requestingUserId,
    string updatingUserId,
    IFixture fixture)
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>
      {
        [JwtRegisteredClaimNames.Sub] = new(JwtRegisteredClaimNames.Sub, requestingUserId),
      });

    var requestingUser = fixture.Build<User>()
      .With(user => user.IsAdmin, false)
      .Create();
    _mockUserRepository.Setup(repository => repository.GetUserById(requestingUserId))
      .ReturnsAsync(requestingUser);

    var response = await UserEndpoints.UpdateAdmin(updatingUserId,
      new UpdateAdminRequest(true),
      _mockUserRepository.Object,
      _mockUserMapper.Object,
      _mockTokenService.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal((int)HttpStatusCode.Forbidden, problemResult.StatusCode);
  }

  [Theory, AutoData]
  public async Task UpdateAdmin_RequestingUserDoesNotExist_ReturnsForbidden(string requestingUserId,
    string updatingUserId)
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>
      {
        [JwtRegisteredClaimNames.Sub] = new(JwtRegisteredClaimNames.Sub, requestingUserId),
      });

    _mockUserRepository.Setup(repository => repository.GetUserById(requestingUserId))
      .ReturnsAsync((User?)null);

    var response = await UserEndpoints.UpdateAdmin(updatingUserId,
      new UpdateAdminRequest(true),
      _mockUserRepository.Object,
      _mockUserMapper.Object,
      _mockTokenService.Object,
      _httpContext);

    var result = Assert.IsType<Results<Ok<UserDto>, ProblemHttpResult>>(response);
    var problemResult = Assert.IsType<ProblemHttpResult>(result.Result);

    Assert.Equal((int)HttpStatusCode.Forbidden, problemResult.StatusCode);
  }
}
