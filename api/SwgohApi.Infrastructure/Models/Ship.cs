namespace SwgohApi.Infrastructure.Models;

public record Ship : Earnable
{
  public Ship(string id,
    string name,
    List<EarnableLocation> locations,
    Marquee? marquee)
  : base(id, name, locations, marquee)
  {
  }

  public override EarnableType Type => EarnableType.Ship;
}
