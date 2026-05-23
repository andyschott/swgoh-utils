namespace SwgohApi.Infrastructure.Models;

public abstract record Earnable
{
  protected Earnable(string id,
    string name,
    List<EarnableLocation> locations,
    Marquee? marquee)
  {
    Id = id;
    Name = name;
    Locations = locations;
    Marquee = marquee;
  }

  public abstract EarnableType Type { get; }

  public string Id { get; init; }
  public string Name { get; init; }
  public List<EarnableLocation> Locations { get; set; }
  public Marquee? Marquee { get; set; }
}
