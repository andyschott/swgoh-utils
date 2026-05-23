namespace SwgohApi.Infrastructure.Models;

public class Marquee
{
  public string Id { get; set; } = string.Empty;

  public string? CharacterId { get; set; }
  public Character? Character { get; set; }

  public string? ShipId { get; set; }
  public Ship? Ship { get; set; }

  public DateOnly IntroductionDate { get; set; }
  public DateOnly MarqueeEventDate { get; set; }
  public DateOnly ShipmentDate { get; set; }
  public DateOnly FarmDate { get; set; }
  public DateOnly? AccelerationDate { get; set; }
}
