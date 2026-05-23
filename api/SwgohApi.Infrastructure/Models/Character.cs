namespace SwgohApi.Infrastructure.Models;

public record Character: Earnable
{
  public Character(string id,
    string name,
    List<EarnableLocation> locations,
    bool isAccelerated,
    Marquee? marquee)
  : base(id, name, locations, marquee)
  {
    IsAccelerated = isAccelerated;
  }

  public override EarnableType Type => EarnableType.Character;

  public bool IsAccelerated { get; set; }
}
