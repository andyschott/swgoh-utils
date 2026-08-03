namespace SwgohApi.ViewModels;

public interface ITableViewModel
{
  IEnumerable<string> Columns { get; }
  IEnumerable<string> Items { get; }

  string GetText(string id, string column);
}
