using System.Security.Claims;
using SwgohApi.Models.Auth;

namespace SwgohApi.Services;

public interface IAuthService
{
  Task<ClaimsPrincipal?> Login(string email, string password);
  Task<TokenResponse?> Login(LoginRequest request);
  Task<TokenResponse?> Refresh(RefreshRequest request);
  Task Revoke(RevokeRequest request);
  Task RevokeAll(string userId);
}
