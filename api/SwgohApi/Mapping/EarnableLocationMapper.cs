using SwgohApi.Models.Earnables;

namespace SwgohApi.Mapping;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

public class EarnableLocationMapper : IMapper<InternalEarnableLocation, EarnableLocation>
{
  public EarnableLocation MapTo(InternalEarnableLocation source)
  {
    return source switch
    {
      InternalEarnableLocation.MarqueeEvent => EarnableLocation.MarqueeEvent,
      InternalEarnableLocation.CrystalShipments => EarnableLocation.CrystalShipments,
      InternalEarnableLocation.DarkSide => EarnableLocation.DarkSide,
      InternalEarnableLocation.LightSide => EarnableLocation.LightSide,
      InternalEarnableLocation.Cantina => EarnableLocation.Cantina,
      InternalEarnableLocation.Fleet => EarnableLocation.Fleet,
      InternalEarnableLocation.CantinaShipments => EarnableLocation.CantinaShipments,
      InternalEarnableLocation.GuildTokenShipments => EarnableLocation.GuildTokenShipments,
      InternalEarnableLocation.RaidMark1Shipments => EarnableLocation.RaidMark1Shipments,
      InternalEarnableLocation.RaidMark2Shipments => EarnableLocation.RaidMark2Shipments,
      InternalEarnableLocation.SquadArenaShipments => EarnableLocation.SquadArenaShipments,
      InternalEarnableLocation.GalacticWarShipments => EarnableLocation.GalacticWarShipments,
      InternalEarnableLocation.FleetArenaShipments => EarnableLocation.FleetArenaShipments,
      InternalEarnableLocation.GuildEventMark1Shipments => EarnableLocation.GuildEventMark1Shipments,
      InternalEarnableLocation.GuildEventMark2Shipments => EarnableLocation.GuildEventMark2Shipments,
      InternalEarnableLocation.GuildEventMark3Shipments => EarnableLocation.GuildEventMark3Shipments,
      InternalEarnableLocation.ShardShopCurrency => EarnableLocation.ShardShopCurrency,
      InternalEarnableLocation.LegendTokens => EarnableLocation.LegendTokens,
      InternalEarnableLocation.ConquestMainReward => EarnableLocation.ConquestMainReward,
      InternalEarnableLocation.ConquestSecondaryReward => EarnableLocation.ConquestSecondaryReward,
      InternalEarnableLocation.ConquestShipments => EarnableLocation.ConquestShipments,
      InternalEarnableLocation.ProvingGrounds => EarnableLocation.ProvingGrounds,
      InternalEarnableLocation.JourneyGuide => EarnableLocation.JourneyGuide,
      _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
    };
  }

  public InternalEarnableLocation MapFrom(EarnableLocation source)
  {
    return source switch
    {
      EarnableLocation.MarqueeEvent => InternalEarnableLocation.MarqueeEvent,
      EarnableLocation.CrystalShipments => InternalEarnableLocation.CrystalShipments,
      EarnableLocation.DarkSide => InternalEarnableLocation.DarkSide,
      EarnableLocation.LightSide => InternalEarnableLocation.LightSide,
      EarnableLocation.Cantina => InternalEarnableLocation.Cantina,
      EarnableLocation.Fleet => InternalEarnableLocation.Fleet,
      EarnableLocation.CantinaShipments => InternalEarnableLocation.CantinaShipments,
      EarnableLocation.GuildTokenShipments => InternalEarnableLocation.GuildTokenShipments,
      EarnableLocation.RaidMark1Shipments => InternalEarnableLocation.RaidMark1Shipments,
      EarnableLocation.RaidMark2Shipments => InternalEarnableLocation.RaidMark2Shipments,
      EarnableLocation.SquadArenaShipments => InternalEarnableLocation.SquadArenaShipments,
      EarnableLocation.GalacticWarShipments => InternalEarnableLocation.GalacticWarShipments,
      EarnableLocation.FleetArenaShipments => InternalEarnableLocation.FleetArenaShipments,
      EarnableLocation.GuildEventMark1Shipments => InternalEarnableLocation.GuildEventMark1Shipments,
      EarnableLocation.GuildEventMark2Shipments => InternalEarnableLocation.GuildEventMark2Shipments,
      EarnableLocation.GuildEventMark3Shipments => InternalEarnableLocation.GuildEventMark3Shipments,
      EarnableLocation.ShardShopCurrency => InternalEarnableLocation.ShardShopCurrency,
      EarnableLocation.LegendTokens => InternalEarnableLocation.LegendTokens,
      EarnableLocation.ConquestMainReward => InternalEarnableLocation.ConquestMainReward,
      EarnableLocation.ConquestSecondaryReward => InternalEarnableLocation.ConquestSecondaryReward,
      EarnableLocation.ConquestShipments => InternalEarnableLocation.ConquestShipments,
      EarnableLocation.ProvingGrounds => InternalEarnableLocation.ProvingGrounds,
      EarnableLocation.JourneyGuide => InternalEarnableLocation.JourneyGuide,
      _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
    };
  }
}
