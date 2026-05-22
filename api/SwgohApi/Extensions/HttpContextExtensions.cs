using System.Diagnostics.CodeAnalysis;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Extensions;

public static class HttpContextExtensions
{
  private const string RequestingUserKey = "RequestingUser";

  extension(HttpContext httpContext)
  {
    [DisallowNull]
    public User? RequestingUser
    {
      get
      {
        if (!httpContext.Items.TryGetValue(RequestingUserKey, out var requestingUser) )
        {
          return null;
        }

        return requestingUser as User;
      }
      set => httpContext.Items[RequestingUserKey] = value;
    }
  }
}
