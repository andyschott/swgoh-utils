using SwgohApi.Infrastructure.Models;
using SwgohApi.Models.Earnables;
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
using InternalConquestRewardPhase = SwgohApi.Infrastructure.Models.ConquestRewardPhase;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;
using Marquee = SwgohApi.Models.Earnables.Marquee;
using MarqueeDate = SwgohApi.Models.Earnables.MarqueeDate;
using Ship = SwgohApi.Models.Earnables.Ship;
using ConquestRewardPhase = SwgohApi.Models.Earnables.ConquestRewardPhase;
using ConquestReward = SwgohApi.Models.Earnables.ConquestReward;

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
      .AddSingleton<IMapper<InternalFarmingStatus, FarmingStatus>, FarmingStatusMapper>()
      .AddSingleton<IMapper<InternalConquestRewardPhase, ConquestRewardPhase>, ConquestRewardPhaseMapper>()
      .AddSingleton<IMapper<InternalConquestReward, ConquestReward>, ConquestRewardMapper>()
      .AddSingleton<IMapper<InternalConquestReward, ConquestRewardDate>, ConquestRewardDateMapper>();
  }
}
