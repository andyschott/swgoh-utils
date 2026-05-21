using SwgohApi.Models.Earnables;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Mapping;

public static class ServiceConfiguration
{
  public static IServiceCollection AddMappers(this IServiceCollection services)
  {
    return services.AddSingleton<IMapper<InternalEarnableLocation, EarnableLocation>, EarnableLocationMapper>()
      .AddSingleton<IMapper<InternalCharacter, Character>, CharacterMapper>()
      .AddSingleton<IMapper<InternalShip, Ship>, ShipMapper>();
  }
}
