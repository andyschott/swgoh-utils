using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;

namespace SwgohApi.Services;

public class EarnableLocationsMapper : IMapper<IEnumerable<EarnableLocation>, string>
{
  private static readonly Dictionary<EarnableLocation, string> _locations = new()
  {
    [EarnableLocation.MarqueeEvent] = "Marquee Event",
    [EarnableLocation.CrystalShipments] = "Shipments",
    [EarnableLocation.DarkSide] = "Normal - Dark Side",
    [EarnableLocation.LightSide] = "Normal - Light Side",
    [EarnableLocation.Cantina] = "Cantina",
    [EarnableLocation.Fleet] = "Fleet",
    [EarnableLocation.CantinaShipments] = "Cantina Shipments",
    [EarnableLocation.GuildTokenShipments] = "Guild Shipments",
    [EarnableLocation.RaidMark1Shipments] = "Raid Mark 1",
    [EarnableLocation.RaidMark2Shipments] = "Raid Mark 2",
    [EarnableLocation.SquadArenaShipments] = "Squad Arena Shipments",
    [EarnableLocation.GalacticWarShipments] = "Galactic War Shipments",
    [EarnableLocation.FleetArenaShipments] = "Fleet Arena Shipments",
    [EarnableLocation.GuildEventMark1Shipments] = "Guild Event Mark 1",
    [EarnableLocation.GuildEventMark2Shipments] = "Guild Event Mark 2",
    [EarnableLocation.GuildEventMark3Shipments] = "Guild Event Mark 3",
    [EarnableLocation.ShardShopCurrency] =  "Shard Shop Currency",
    [EarnableLocation.LegendTokens] =  "Legend Tokens",
    [EarnableLocation.ConquestMainReward] = "Conquest Main Reward",
    [EarnableLocation.ConquestSecondaryReward] = "Conquest Secondary Reward",
    [EarnableLocation.ConquestShipments] = "Conquest Shipments",
    [EarnableLocation.ProvingGrounds] = "Proving Grounds",
    [EarnableLocation.JourneyGuide] = "Journey Guide",
  };

  public string MapTo(IEnumerable<EarnableLocation> source)
  {
    var descriptions = source.Select(location => _locations[location])
      .Order();
    return string.Join(", ", descriptions);
  }

  public IEnumerable<EarnableLocation> MapFrom(string destination)
  {
    throw new NotImplementedException();
  }
}
