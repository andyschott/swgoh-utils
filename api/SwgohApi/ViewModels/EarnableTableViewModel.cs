using SwgohApi.Models.Earnables;
using SwgohApi.Services;

namespace SwgohApi.ViewModels;

public abstract class EarnableTableViewModel<T> : ITableViewModel
where T : Earnable
{
  protected readonly Dictionary<string, T> _earnables;

  protected const string NameColumn = "Name";
  protected const string LocationsColumn = "Location";

  private static EarnableLocationsMapper _locationsMapper = new();

  protected EarnableTableViewModel(IEnumerable<T> earnables)
  {
    _earnables = earnables.ToDictionary(earnable => earnable.Id);
  }

  public abstract string Title { get; }

  public virtual IEnumerable<string> Columns { get; } = [NameColumn, LocationsColumn];

  public IEnumerable<string> Items => _earnables.Keys;

  public string DefaultSortColumn => NameColumn;
  public bool DefaultSortAscending => true;

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
      LocationsColumn => _locationsMapper.MapTo(earnable.Locations),
      _ => throw new ArgumentOutOfRangeException(nameof(column), column)
    };
  }

  protected virtual string? GetText(T earnable, string column) => null;
}
