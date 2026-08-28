using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public class UserShipsTableViewModel : UserEarnablesTableViewModel<Ship>
{
  public UserShipsTableViewModel(IEnumerable<Ship> earnables)
    : base(earnables)
  {
  }

  public override string Title => "Your Ships";
}
