namespace SwgohApi.Models.Earnables;

public record CreateCharacterMarqueeRequest(DateOnly IntroductionDate,
  DateOnly MarqueeEventDate,
  DateOnly ShipmentDate,
  DateOnly FarmDate,
  DateOnly AccelerationDate);
