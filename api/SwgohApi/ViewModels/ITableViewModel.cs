namespace SwgohApi.ViewModels;

public interface ITableViewModel
{
  string Title { get; }

  IEnumerable<string> Columns { get; }
  IEnumerable<string> Items { get; }

  string DefaultSortColumn { get; }
  bool DefaultSortAscending { get; }

  string GetText(string id, string column);

  string GetDataType(string column) => "text";
  string? GetCellClass(string id, string column) => string.Empty;
  string? GetCellToolip(string id, string column) => string.Empty;
  string GetFilterPlaceHolder() => "Enter a name";
}
