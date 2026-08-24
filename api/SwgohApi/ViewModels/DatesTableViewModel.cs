namespace SwgohApi.ViewModels;

public abstract class DatesTableViewModel : ITableViewModel
{
  public abstract string Title { get; }
  public abstract IEnumerable<string> Columns { get; }
  public abstract IEnumerable<string> Items { get; }
  public abstract string DefaultSortColumn { get; }
  public abstract bool DefaultSortAscending { get; }
  public abstract string GetText(string id, string column);

  public virtual string GetDataType(string column) => "text";

  public string? GetCellClass(string id, string column)
  {
    var isEstimated = IsEstimated(id, column);
    return isEstimated ? "estimate" : string.Empty;
  }

  public string? GetCellToolip(string id, string column)
  {
    var isEstimated = IsEstimated(id, column);
    return isEstimated ? "Estimated" : string.Empty;
  }

  private bool IsEstimated(string id, string column)
  {
    var date = GetDateForColumn(id, column);

    var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);
    return date > today;
  }

  protected abstract DateOnly? GetDateForColumn(string id, string column);
}
