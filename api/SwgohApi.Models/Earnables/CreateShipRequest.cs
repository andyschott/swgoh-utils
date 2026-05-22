namespace SwgohApi.Models.Earnables;

public record CreateShipRequest(string Name,
  EarnableLocation[] Locations);
