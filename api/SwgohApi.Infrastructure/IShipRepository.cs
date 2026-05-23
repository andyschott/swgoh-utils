using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface IShipRepository
{
  Task<Ship> CreateShip(string name,
    IEnumerable<EarnableLocation> locations);
  Task<Ship?> GetShipByName(string name);
  Task<IEnumerable<Ship>> GetShips();
  Task<Ship?> GetShip(string id);
}
