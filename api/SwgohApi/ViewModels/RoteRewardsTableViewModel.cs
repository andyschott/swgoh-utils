using SwgohApi.Models.TerritoryBattles;

namespace SwgohApi.ViewModels;

public class RoteRewardsTableViewModel : ITableViewModel
{
  private readonly Dictionary<string, RiseOfTheEmpireRewards> _rewards;

  private const string StarsColumn = "Stars";
  private const string Get1Column = "GET1";
  private const string Get2Column = "GET2";
  private const string Get3Column = "GET3";
  private const string CrystalsColumn = "Crystals";
  private const string FragmentedSignalDataColumn = "Fragmented Signal Data";
  private const string IncompleteSignalDataColumn = "Incomplete Signal Data";
  private const string FlawedSignalDataColumn = "Flawed Signal Data";
  private const string AudoriumHeatsinksColumn = "Audorium Heatsinks";
  private const string ElectriumConductorsColumn = "Electrium Conductors";
  private const string ZinbiddleCardsColumn = "Zinbiddle Cards";
  private const string ImpulseDetectorsColumn = "Impulse Detectors";
  private const string GyrdaKeypadsColumn = "Gyrd Key Pads";
  private const string FinishersColumn = "Finishers";
  private const string KyroKeypadsColumn = "Kyro Keypads";
  private const string KyroShockProdsColumn = "Kyro Shock Prods";
  private const string Gear12PlusMainColumn = "Gear 12+ Main";
  private const string Gear12PlusSecondaryColumn = "Gear 12+ Secondary";
  private const string Gear12Column = "Gear 12";
  private const string Gear12PurpleColumn = "Gear 12 Purple";
  private const string CoreGearColumn = "Core Gear";

  private const string UnknownValue = "?";

  public RoteRewardsTableViewModel(RiseOfTheEmpireRewards[] rewards)
  {
    _rewards = rewards.ToDictionary(reward => reward.Stars.ToString());
  }

  public string Title { get; } = "Rise of the Empire Rewards";

  public IEnumerable<string> Columns { get; } =
  [
    StarsColumn,
    Get1Column,
    Get2Column,
    Get3Column,
    CrystalsColumn,
    FragmentedSignalDataColumn,
    IncompleteSignalDataColumn,
    FlawedSignalDataColumn,
    AudoriumHeatsinksColumn,
    ElectriumConductorsColumn,
    ZinbiddleCardsColumn,
    ImpulseDetectorsColumn,
    GyrdaKeypadsColumn,
    FinishersColumn,
    KyroKeypadsColumn,
    KyroShockProdsColumn,
    Gear12PlusMainColumn,
    Gear12PlusSecondaryColumn,
    Gear12Column,
    Gear12PurpleColumn,
    CoreGearColumn
  ];

  public IEnumerable<string> Items => _rewards.Keys;

  public string DefaultSortColumn => StarsColumn;

  public bool DefaultSortAscending => false;

  public string GetText(string id, string column)
  {
    var rewards = _rewards[id];
    return column switch
    {
      StarsColumn => rewards.Stars.ToString(),
      Get1Column => FormatNumber(rewards.GuildEventCurrencyMk1),
      Get2Column => FormatNumber(rewards.GuildEventCurrencyMk2),
      Get3Column => FormatNumber(rewards.GuildEventCurrencyMk3),
      CrystalsColumn => rewards.Crystals?.ToString() ?? string.Empty,
      FragmentedSignalDataColumn => FormatNumber(rewards.FragmentedSignalData),
      IncompleteSignalDataColumn => FormatNumber(rewards.IncompleteSignalData),
      FlawedSignalDataColumn => FormatNumber(rewards.FlawedSignalData),
      AudoriumHeatsinksColumn => FormatNumber(rewards.AurodiumHeatSinks),
      ElectriumConductorsColumn => FormatNumber(rewards.ElectriumConductors),
      ZinbiddleCardsColumn => FormatNumber(rewards.ZinbiddleCards),
      ImpulseDetectorsColumn => FormatNumber(rewards.ImpulseDetectors),
      GyrdaKeypadsColumn => FormatNumber(rewards.GyrdaKeypads),
      FinishersColumn => FormatArray(rewards.Finishers),
      KyroKeypadsColumn => FormatNumber(rewards.KyroKeypads),
      KyroShockProdsColumn => FormatNumber(rewards.KyroShockProds),
      Gear12PlusMainColumn => FormatArray(rewards.Gear12PlusMain),
      Gear12PlusSecondaryColumn => FormatArray(rewards.Gear12PlusSecondary),
      Gear12Column => FormatArray(rewards.Gear12),
      Gear12PurpleColumn => FormatArray(rewards.Gear12Purple),
      CoreGearColumn => FormatArray(rewards.CoreGear),
      _ => throw new ArgumentOutOfRangeException(nameof(column), column)
    };
  }

  public string GetFilterPlaceHolder() => "Enter stars";

  private static string FormatNumber(int? value)
  {
    if (value is null)
    {
      return UnknownValue;
    }

    return value.Value.ToString();
  }

  private static string FormatArray(int[]? values)
  {
    if (values is null)
    {
      return UnknownValue;
    }

    return string.Join(" + ", values);
  }
}
