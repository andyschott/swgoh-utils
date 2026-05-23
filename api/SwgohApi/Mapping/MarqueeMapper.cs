using SwgohApi.Models.Earnables;

namespace SwgohApi.Mapping;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

public class MarqueeMapper : IMapper<InternalMarquee, Marquee>
{
  public Marquee MapTo(InternalMarquee source)
  {
    return new Marquee(source.Id,
      source.IntroductionDate,
      source.MarqueeEventDate,
      source.ShipmentDate,
      source.FarmDate,
      source.AccelerationDate);
  }

  public InternalMarquee MapFrom(Marquee destination)
  {
    return new InternalMarquee(destination.Id,
      null,
      null,
      destination.IntroductionDate,
      destination.MarqueeEventDate,
      destination.ShipmentDate,
      destination.FarmDate,
      destination.AccelerationDate);
  }
}
