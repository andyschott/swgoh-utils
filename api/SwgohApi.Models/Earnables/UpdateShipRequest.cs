namespace SwgohApi.Models.Earnables;

public record UpdateShipRequest(EarnableLocation[]? Locations,
  MarqueeRequest? Marquee);
