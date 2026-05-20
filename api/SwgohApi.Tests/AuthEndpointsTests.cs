using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Auth;
using SwgohApi.Services;

namespace SwgohApi.Tests;

public class AuthEndpointsTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);
  private readonly Mock<IAuthService> _mockAuthService;

  public AuthEndpointsTests()
  {
    _mockAuthService = _mockRepository.Create<IAuthService>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, AutoData]
  public async Task Login_Successful(LoginRequest request,
    TokenResponse tokenResponse)
  {
    _mockAuthService.Setup(x => x.Login(request))
      .ReturnsAsync(tokenResponse);

    var response = await AuthEndpoints.Login(request, _mockAuthService.Object);

    var result = Assert.IsType<Results<Ok<TokenResponse>, UnauthorizedHttpResult>>(response);
    var okResult = Assert.IsType<Ok<TokenResponse>>(result.Result);
    Assert.Same(tokenResponse, okResult.Value);
  }

  [Theory, AutoData]
  public async Task Login_InvalidCredentials_ReturnsUnauthorized(LoginRequest request)
  {
    _mockAuthService.Setup(x => x.Login(request))
      .ReturnsAsync((TokenResponse?)null);

    var response = await AuthEndpoints.Login(request, _mockAuthService.Object);

    var result = Assert.IsType<Results<Ok<TokenResponse>, UnauthorizedHttpResult>>(response);
    Assert.IsType<UnauthorizedHttpResult>(result.Result);
  }

  [Fact]
  public async Task RevokeAll_UserIdMissing_ReturnsUnauthorized()
  {
    var context = new DefaultHttpContext();

    var response = await AuthEndpoints.RevokeAll(context, _mockAuthService.Object);

    var result = Assert.IsType<Results<Ok, UnauthorizedHttpResult>>(response);
    Assert.IsType<UnauthorizedHttpResult>(result.Result);
  }

  [Theory, AutoData]
  public async Task RevokeAll_Successful(string userId)
  {
    _mockAuthService.Setup(x => x.RevokeAll(userId))
      .Returns(Task.CompletedTask);
    var context = new DefaultHttpContext
    {
      User = new ClaimsPrincipal(new ClaimsIdentity(
      [
        new Claim(JwtRegisteredClaimNames.Sub, userId)
      ], "test"))
    };

    var response = await AuthEndpoints.RevokeAll(context, _mockAuthService.Object);

    var result = Assert.IsType<Results<Ok, UnauthorizedHttpResult>>(response);
    Assert.IsType<Ok>(result.Result);
  }
}
