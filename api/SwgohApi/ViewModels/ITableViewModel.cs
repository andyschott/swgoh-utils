namespace SwgohApi.ViewModels;

public interface ITableViewModel
{
  string Title { get; }

  IEnumerable<string> Columns { get; }
  IEnumerable<string> Items { get; }

  string GetText(string id, string column);

  string? GetCellClass(string id, string column) => string.Empty;
  string? GetCellToolip(string id, string column) => string.Empty;
}
