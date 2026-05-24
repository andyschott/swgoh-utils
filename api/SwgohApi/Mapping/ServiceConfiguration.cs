using SwgohApi.Infrastructure.Models;
using SwgohApi.Models.Users;
using Character = SwgohApi.Models.Earnables.Character;
using EarnableLocation = SwgohApi.Models.Earnables.EarnableLocation;
using EarnableShards =  SwgohApi.Models.Earnables.EarnableShards;
using FarmingStatus = SwgohApi.Models.Earnables.FarmingStatus;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalEarnableShards = SwgohApi.Infrastructure.Models.EarnableShards;
using InternalFarmingStatus = SwgohApi.Infrastructure.Models.FarmingStatus;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using Marquee = SwgohApi.Models.Earnables.Marquee;
using MarqueeDate = SwgohApi.Models.Earnables.MarqueeDate;
using Ship = SwgohApi.Models.Earnables.Ship;

namespace SwgohApi.Mapping;

public static class ServiceConfiguration
{
  public static IServiceCollection AddMappers(this IServiceCollection services)
  {
    return services.AddSingleton<IMapper<InternalEarnableLocation, EarnableLocation>, EarnableLocationMapper>()
      .AddSingleton<IMapper<InternalCharacter, Character>, CharacterMapper>()
      .AddSingleton<IMapper<InternalShip, Ship>, ShipMapper>()
      .AddSingleton<IMapper<InternalMarquee, Marquee>, MarqueeMapper>()
      .AddSingleton<IMapper<InternalMarquee, MarqueeDate>, MarqueeDateMapper>()
      .AddSingleton<IMapper<User, UserDto>, UserMapper>()
      .AddSingleton<IMapper<InternalEarnableShards,  EarnableShards>, EarnableShardsMapper>()
      .AddSingleton<IMapper<InternalFarmingStatus, FarmingStatus>, FarmingStatusMapper>();
  }
}
