using System.Security.Claims;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using SwgohApi.Extensions;
using SwgohApi.Infrastructure;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Middleware;
using SwgohApi.Services;
using SwgohApi.Tests.Extensions;

namespace SwgohApi.Tests.Middleware;

public sealed class RequestingUserMiddlewareTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<ITokenService> _mockTokenService;
  private readonly Mock<IUserRepository> _mockUserRepository;

  private readonly Mock<RequestDelegate> _mockNext;

  private readonly HttpContext _httpContext = new DefaultHttpContext();

  private readonly RequestingUserMiddleware _middleware;

  public RequestingUserMiddlewareTests()
  {
    _mockTokenService = _mockRepository.Create<ITokenService>();
    _mockUserRepository = _mockRepository.Create<IUserRepository>();

    _mockNext = _mockRepository.Create<RequestDelegate>();

    _middleware = new RequestingUserMiddleware(_mockNext.Object);
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Fact]
  public async Task Invoke_NoRequestingUser_DoesNothing()
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(new Dictionary<string, Claim>());

    _mockNext.Setup(next => next(_httpContext))
      .Returns(Task.CompletedTask);

    await _middleware.Invoke(_httpContext,
      _mockTokenService.Object,
      _mockUserRepository.Object);

    Assert.Null(_httpContext.RequestingUser);
  }
  [Theory, AutoData]
  public async Task Invoke_RequestingUserNotFound_ReturnsUnauthorized(string userId,
    IFixture fixture)
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(fixture.CreateClaims(userId));
    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync((User?)null);

    await _middleware.Invoke(_httpContext,
      _mockTokenService.Object,
      _mockUserRepository.Object);

    Assert.Null(_httpContext.RequestingUser);
    Assert.Equal(StatusCodes.Status401Unauthorized,
      _httpContext.Response.StatusCode);
  }

  [Theory, AutoData]
  public async Task Invoke_RequestingUserFound_AddsToContext(string userId,
    User user,
    IFixture fixture)
  {
    _mockTokenService.Setup(service => service.GetClaims(_httpContext))
      .ReturnsAsync(fixture.CreateClaims(userId));
    _mockUserRepository.Setup(repository => repository.GetUserById(userId))
      .ReturnsAsync(user);

    _mockNext.Setup(next => next(_httpContext))
      .Returns(Task.CompletedTask);

    await _middleware.Invoke(_httpContext,
      _mockTokenService.Object,
      _mockUserRepository.Object);

    Assert.Same(user, _httpContext.RequestingUser);
  }
}
