namespace SwgohApi.Infrastructure.Models;

public abstract class Earnable
{
  public abstract EarnableType Type { get; }

  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public List<EarnableLocation> Locations { get; set; } = [];
  public Marquee? Marquee { get; set; }
  public List<EarnableShards> EarnableShards { get; set; } = [];
}
