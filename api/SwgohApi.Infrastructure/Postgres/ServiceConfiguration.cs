using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SwgohApi.Infrastructure.Postgres;

public static class ServiceConfiguration
{
  public static IServiceCollection AddPostgres(this IServiceCollection services,
    PostgresConfiguration configuration)
  {
    Validator.ValidateObject(configuration,
      new ValidationContext(configuration));

    services.AddDbContextPool<PostgresDbContext>(options =>
    {
      options.UseNpgsql(configuration.ConnectionString,
        postgresOptions =>
        {
          postgresOptions.SetPostgresVersion(18, 0);
        });
    });
    services.AddScoped<IPostgresDbContext>(provider =>
      provider.GetRequiredService<PostgresDbContext>());

    services.AddScoped<IUserRepository, PostgresUserRepository>()
      .AddScoped<ITokenRepository, PostgresTokenRepository>()
      .AddScoped<ICharacterRepository, PostgresCharacterRepository>()
      .AddScoped<IShipRepository, PostgresShipRepository>()
      .AddScoped<IMarqueeRepository, PostgresMarqueeRepository>();

    return services;
  }
}
