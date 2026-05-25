using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Extensions;
using SwgohApi.Filters;
using SwgohApi.Infrastructure.Models;
using SwgohApi.TestUtilities;

namespace SwgohApi.Tests.Filters;

public sealed class RequireAdminEndpointFilterTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<EndpointFilterInvocationContext> _mockFilterContext;
  private readonly Mock<EndpointFilterDelegate> _mockNext;

  private readonly HttpContext _httpContext = new DefaultHttpContext();

  private readonly RequireAdminEndpointFilter _filter = new();

  public RequireAdminEndpointFilterTests()
  {
    _mockFilterContext = _mockRepository.Create<EndpointFilterInvocationContext>();
    _mockFilterContext.Setup(context => context.HttpContext)
      .Returns(_httpContext);
    _mockNext = _mockRepository.Create<EndpointFilterDelegate>();
  }

  [Fact]
  public async Task InvokeAsync_UserNotInContext_ReturnsForbidden()
  {
    var result = await _filter.InvokeAsync(_mockFilterContext.Object,
      _mockNext.Object);

    Assert.IsType<ForbidHttpResult>(result);
  }

  [Theory, SwgohApiAutoData]
  public async Task InvokeAsync_UserNotAdmin_ReturnsForbidden(IFixture fixture)
  {
    var user = fixture.Build<User>()
      .With(user => user.IsAdmin, false)
      .With(user => user.EarnableShards, [])
      .Create();
    _httpContext.RequestingUser = user;

    var result = await _filter.InvokeAsync(_mockFilterContext.Object,
      _mockNext.Object);

    Assert.IsType<ForbidHttpResult>(result);
  }

  [Theory, SwgohApiAutoData]
  public async Task InvokeAsync_UserIsAdmin_CallsNext(IFixture fixture)
  {
    var user = fixture.Build<User>()
      .With(user => user.IsAdmin, true)
      .With(user => user.EarnableShards, [])
      .Create();
    _httpContext.RequestingUser = user;

    var mockResult = _mockRepository.Create<IResult>();
    _mockNext.Setup(next => next(_mockFilterContext.Object))
      .ReturnsAsync(mockResult.Object);

    var result = await _filter.InvokeAsync(_mockFilterContext.Object,
      _mockNext.Object);

    Assert.Same(mockResult.Object, result);
  }

  public void Dispose() => _mockRepository.VerifyAll();
}
