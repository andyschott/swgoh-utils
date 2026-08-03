using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public class ShipsTableViewModel : EarnableTableViewModel<Ship>
{
  public ShipsTableViewModel(IEnumerable<Ship> earnables)
    : base(earnables)
  {
  }
}
