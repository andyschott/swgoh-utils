using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public abstract class UserEarnablesTableViewModel<T> : ITableViewModel
where T : Earnable
{
  private const string NameColumn = "Name";
  private const string StarsColumn = "Stars";
  private const string ShardsColumn = "Shards";
  private const string RemainingShardsColumn = "Shards Remaining";
  private const string StatusColumn = "Status";
  private const string LocationsColumn = "Loctions";

  private readonly Dictionary<string, T> _earnables;

  public UserEarnablesTableViewModel(IEnumerable<T> earnables)
  {
    _earnables = earnables.ToDictionary(earnable => earnable.Id);
  }

  public abstract string Title { get; }

  public IEnumerable<string> Columns { get; } =
  [
    NameColumn,
    StarsColumn,
    ShardsColumn,
    RemainingShardsColumn,
    StatusColumn,
    LocationsColumn,
  ];
  public IEnumerable<string> Items => _earnables.Keys;

  public string DefaultSortColumn => StatusColumn;
  public bool DefaultSortAscending => true;

  public string GetText(string id, string column)
  {
    var earnable = _earnables[id];
    var (stars, shards) = ConvertFromTotalShards(earnable.Shards!.Shards);
    return column switch
    {
      NameColumn => earnable.Name,
      StarsColumn => stars.ToString(),
      ShardsColumn => shards.ToString(),
      RemainingShardsColumn => (330 - earnable.Shards.Shards).ToString(),
      StatusColumn => earnable.Shards.FarmingStatus.ToString(),
      // TODO: map locations
      _ => string.Empty
    };
  }

  private static (int Stars, int Shards) ConvertFromTotalShards(int shards)
  {
    return shards switch
    {
      < 10 => (0, shards),
      < 25 => (1, shards - 10),
      < 50 => (2, shards - 25),
      < 80 => (3, shards - 50),
      < 145 => (4, shards - 80),
      < 230 => (5, shards - 145),
      < 330 => (6, shards - 230),
      _ => (7, 0)
    };
  }
}
