using Microsoft.Extensions.DependencyInjection;

namespace SwgohApi.Infrastructure.Utilities;

public static class ServiceConfiguration
{
  public static IServiceCollection AddUtilityServices(this IServiceCollection services)
  {
    services.AddSingleton<IIdGenerator, GuidIdGenerator>();
    return services;
  }
}
