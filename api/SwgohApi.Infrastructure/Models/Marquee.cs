namespace SwgohApi.Infrastructure.Models;

public record Marquee
{
  public string Id { get; init; }

  public string? CharacterId { get; set; }
  public Character? Character { get; set; }

  public string? ShipId { get; set; }
  public Ship? Ship { get; set; }

  public DateOnly IntroductionDate { get; init; }
  public DateOnly MarqueeEventDate { get; init; }
  public DateOnly ShipmentDate { get; set; }
  public DateOnly FarmDate { get; set; }
  public DateOnly? AccelerationDate { get; set; }

  public Marquee(string id,
    string? characterId,
    string? shipId,
    DateOnly introductionDate,
    DateOnly marqueeEventDate,
    DateOnly shipmentDate,
    DateOnly farmDate,
    DateOnly? accelerationDate)
  {
    Id = id;

    CharacterId = characterId;
    Character = null;

    ShipId = shipId;
    Ship = null;

    IntroductionDate = introductionDate;
    MarqueeEventDate = marqueeEventDate;
    ShipmentDate = shipmentDate;
    FarmDate = farmDate;
    AccelerationDate = accelerationDate;
  }
}
