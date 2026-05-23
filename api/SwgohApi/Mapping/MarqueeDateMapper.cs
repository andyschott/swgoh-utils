using SwgohApi.Models.Earnables;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Mapping;

public class MarqueeDateMapper : IMapper<InternalMarquee, MarqueeDate>
{
  public MarqueeDate MapTo(InternalMarquee source)
  {
    var name = source.Character?.Name ?? source.Ship?.Name;
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Marquee must be associated with a Character or Ship name.", nameof(source));
    }

    return new MarqueeDate(name,
      source.IntroductionDate,
      source.MarqueeEventDate,
      source.ShipmentDate,
      source.FarmDate,
      source.AccelerationDate);
  }

  public InternalMarquee MapFrom(MarqueeDate destination)
  {
    throw new NotImplementedException();
  }
}
