using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public abstract class EarnableTableViewModel<T> : ITableViewModel
where T : Earnable
{
  protected readonly Dictionary<string, T> _earnables;

  protected const string NameColumn = "Name";
  protected const string LocationsColumn = "Location";

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

  protected EarnableTableViewModel(IEnumerable<T> earnables)
  {
    _earnables = earnables.ToDictionary(earnable => earnable.Id);
  }

  public virtual IEnumerable<string> Columns { get; } = [NameColumn, LocationsColumn];

  public IEnumerable<string> Items => _earnables.Keys;

  public string GetText(string id, string column)
  {
    var earnable = _earnables[id];
    var text = GetText(earnable, column);
    if (!string.IsNullOrEmpty(text))
    {
      return text;
    }

    return column switch
    {
      NameColumn => earnable.Name,
      LocationsColumn => GetLocationsText(earnable.Locations),
      _ => throw new ArgumentOutOfRangeException(nameof(column), column)
    };
  }

  protected abstract string? GetText(T earnable, string column);

  private static string GetLocationsText(IEnumerable<EarnableLocation> locations)
  {
    var descriptions = locations.Select(location => _locations[location])
      .Order();
    return string.Join(", ", descriptions);
  }
}
