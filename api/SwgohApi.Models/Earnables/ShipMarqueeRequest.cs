namespace SwgohApi.Models.Earnables;

public record ShipMarqueeRequest(DateOnly IntroductionDate,
  DateOnly MarqueeEventDate,
  DateOnly ShipmentDate,
  DateOnly FarmDate);
