namespace SwgohApi.Models.Earnables;

public record Marquee(string Id,
  DateOnly IntroductionDate,
  DateOnly MarqueeEventDate,
  DateOnly ShipmentDate,
  DateOnly FarmDate,
  DateOnly? AccelerationDate = null);
