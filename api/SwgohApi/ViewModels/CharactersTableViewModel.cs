using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public class CharactersTableViewModel: EarnableTableViewModel<Character>
{
  private const string AcceleratedColumn = "Accelerated";

  public CharactersTableViewModel(IEnumerable<Character> characters)
  : base(characters)
  {
  }

  public override IEnumerable<string> Columns { get; } = [NameColumn, LocationsColumn, AcceleratedColumn];

  protected override string? GetText(Character character, string column)
  {
    if (column is AcceleratedColumn)
    {
      return character.IsAccelerated ? "Yes" : "No";
    }

    return null;
  }
}
