using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface IEarnableRepository<T>
where T : Earnable
{
  Task<IEnumerable<T>> GetEarnables();
  Task<T?> GetEarnable(string id);
  Task<T?> GetEarnableByName(string name);
  Task SaveEarnable(T earnable);
}
