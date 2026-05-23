using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure;

public interface IMarqueeRepository
{
  Task<Marquee> CreateMarquee(Earnable earnable,
    DateOnly introductionDate,
    DateOnly marqueeEventDate,
    DateOnly shipmentDate,
    DateOnly farmDate,
    DateOnly? accelerationDate);
  Task<IEnumerable<Marquee>> GetMarquees();
  Task<Marquee?> GetMarquee(string id);
  Task SaveMarquee(Marquee marquee);
}
