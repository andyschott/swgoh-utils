namespace SwgohApi.Infrastructure.Models;

public record Ship : Earnable
{
  public Ship(string id,
    string name,
    List<EarnableLocation> locations)
  : base(id, name, locations)
  {
  }

  public override EarnableType Type => EarnableType.Ship;
}
