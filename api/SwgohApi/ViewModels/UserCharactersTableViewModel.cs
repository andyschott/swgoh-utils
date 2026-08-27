using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public class UserCharactersTableViewModel : UserEarnablesTableViewModel<Character>
{
  public UserCharactersTableViewModel(IEnumerable<Character> earnables)
    : base(earnables)
  {
  }

  public override string Title => "Your Characters";
}
