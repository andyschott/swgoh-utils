using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SwgohApi.Infrastructure.Models;

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
      .AddScoped<PostgresCharacterRepository>()
      .AddScoped<PostgresShipRepository>()
      .AddScoped<IMarqueeRepository, PostgresMarqueeRepository>()
      .AddScoped<IEarnableShardsRepository, PostgresEarnableShardsRepository>();

    services.AddScoped<ICharacterRepository>(provider =>
      provider.GetRequiredService<PostgresCharacterRepository>());
    services.AddScoped<IEarnableRepository<Character>>(provider =>
      provider.GetRequiredService<PostgresCharacterRepository>());

    services.AddScoped<IShipRepository>(provider =>
      provider.GetRequiredService<PostgresShipRepository>());
    services.AddScoped<IEarnableRepository<Ship>>(provider =>
      provider.GetRequiredService<PostgresShipRepository>());

    return services;
  }
}
