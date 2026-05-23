namespace SwgohApi.Models.Earnables;

public record MarqueeDate(string Name,
  DateOnly IntroductionDate,
  DateOnly MarqueeEventDate,
  DateOnly ShipmentDate,
  DateOnly FarmDate,
  DateOnly? AccelerationDate = null);
