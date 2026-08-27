using SwgohApi.Models.Earnables;

namespace SwgohApi.ViewModels;

public class UserEarnablesViewModel<T>
where T : Earnable
{
  public required UserEarnablesTableViewModel<T> Earnables { get; set; }
}
