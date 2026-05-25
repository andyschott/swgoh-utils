namespace SwgohApi.Models.Earnables;

public record MarqueeRequest(DateOnly IntroductionDate,
  DateOnly MarqueeEventDate,
  DateOnly ShipmentDate,
  DateOnly FarmDate);
