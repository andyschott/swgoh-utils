using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface IShipRepository : IEarnableRepository<Ship>
{
  Task<Ship> CreateShip(string name,
    IEnumerable<EarnableLocation> locations);
}
