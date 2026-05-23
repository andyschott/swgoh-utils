namespace SwgohApi.Models.Earnables;

public record CharacterMarqueeRequest(DateOnly IntroductionDate,
  DateOnly MarqueeEventDate,
  DateOnly ShipmentDate,
  DateOnly FarmDate,
  DateOnly AccelerationDate);
