using SwgohApi.Models.Earnables;

namespace SwgohApi.Services;

public class UserEarnableComparer : IComparer<Earnable>
{
  public int Compare(Earnable? x, Earnable? y)
  {
    if (ReferenceEquals(x, y))
    {
      return 0;
    }

    if (x is null)
    {
      return 1;
    }

    if (y is null)
    {
      return -1;
    }

    if (x.Shards is null)
    {
      throw new ArgumentException("Shards must not be null", nameof(x));
    }

    if (y.Shards is null)
    {
      throw new ArgumentException("Shards must not be null", nameof(y));
    }

    var xStatus = x.Shards.FarmingStatus;
    var yStatus = y.Shards.FarmingStatus;

    if (xStatus == yStatus)
    {
      return x.Name.CompareTo(y.Name, StringComparison.OrdinalIgnoreCase);
    }

    // Order is Active -> Backlog -> Done
    if (xStatus is FarmingStatus.Active)
    {
      return -1;
    }

    if (yStatus is FarmingStatus.Active)
    {
      return 1;
    }

    if (xStatus is FarmingStatus.Backlog)
    {
      return -1;
    }

    if (yStatus is FarmingStatus.Backlog)
    {
      return 1;
    }

    if (xStatus is FarmingStatus.Done)
    {
      return -1;
    }

    if (yStatus is FarmingStatus.Done)
    {
      return 1;
    }

    return 0;
  }
}
