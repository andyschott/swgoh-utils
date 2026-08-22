using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public class ConquestRewardsTableViewModel : DatesTableViewModel
{
  private readonly Dictionary<string, ConquestRewardDate> _conquestRewards;

  private const string NameColumn = "Name";
  private const string RewardPhaseColumn = "Reward Phase";
  private const string InitialUnlockDateColumn = "Initial Unlock Date";
  private const string FinalRewardCreateDateColumn = "Final Reward Create Date";
  private const string ProvingGroundsDateColumn = "Proving Grounds Date";

  private static readonly Dictionary<ConquestRewardPhase, string> _conquestRewardPhases = new()
  {
    [ConquestRewardPhase.MainReward] = "Main Reward",
    [ConquestRewardPhase.SecondaryReward] = "Secondary Reward",
    [ConquestRewardPhase.ConquestShipments] = "Conquest Shipments",
    [ConquestRewardPhase.ProvingGrounds] = "Proving Grounds",
  };

  public ConquestRewardsTableViewModel(IEnumerable<ConquestRewardDate> conquestRewards)
  {
    _conquestRewards = conquestRewards.ToDictionary(cr => cr.Name);
  }

  public override string Title => "Conquest Rewards";

  public override IEnumerable<string> Columns { get; } =
  [
    NameColumn,
    RewardPhaseColumn,
    InitialUnlockDateColumn,
    FinalRewardCreateDateColumn,
    ProvingGroundsDateColumn
  ];

  public override IEnumerable<string> Items => _conquestRewards.Keys;

  public override string DefaultSortColumn => InitialUnlockDateColumn;
  public override bool DefaultSortAscending => false;

  public override string GetText(string id, string column)
  {
    var conquestReward = _conquestRewards[id];
    return column switch
    {
      NameColumn => conquestReward.Name,
      RewardPhaseColumn => _conquestRewardPhases[conquestReward.RewardPhase],
      InitialUnlockDateColumn => FormatDate(conquestReward.InitialUnlockDate),
      FinalRewardCreateDateColumn => FormatDate(conquestReward.FinalRewardCreateDate),
      ProvingGroundsDateColumn => FormatDate(conquestReward.ProvingGroundsDate),
      _ => throw new ArgumentOutOfRangeException(nameof(column), column)
    };
  }

  public string GetDataType(string column)
  {
    return column switch
    {
      NameColumn => "text",
      RewardPhaseColumn => "text",
      InitialUnlockDateColumn => "date",
      FinalRewardCreateDateColumn => "date",
      ProvingGroundsDateColumn => "date",
      _ => throw new ArgumentOutOfRangeException(nameof(column), column)
    };
  }

  protected override DateOnly? GetDateForColumn(string id, string column)
  {
    if (column is not ProvingGroundsDateColumn)
    {
      return null;
    }

    return _conquestRewards[id].ProvingGroundsDate;
  }

  private static string FormatDate(DateOnly? date)
  {
    if (date is null)
    {
      return string.Empty;
    }

    return date.Value.ToString("d");
  }
}
