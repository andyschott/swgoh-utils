using SwgohApi.Models.Earnables;
using InternalEarnable = SwgohApi.Infrastructure.Models.Earnable;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Mapping;

public abstract class EarnableMapper<TInternalEarnable, TEarnable> : IMapper<TInternalEarnable, TEarnable>
where TInternalEarnable : InternalEarnable
where TEarnable : Earnable
{
  private readonly IMapper<InternalEarnableLocation, EarnableLocation> _locationMapper;

  protected EarnableMapper(IMapper<InternalEarnableLocation, EarnableLocation> locationMapper)
  {
    _locationMapper = locationMapper;
  }

  public TEarnable MapTo(TInternalEarnable source)
  {
    var locations = source.Locations.Select(location => _locationMapper.MapTo(location))
      .ToArray();
    return Create(source, locations);
  }

  public TInternalEarnable MapFrom(TEarnable destination)
  {
    var locations = destination.Locations.Select(location => _locationMapper.MapFrom(location))
      .ToList();
    return Create(destination, locations);
  }

  protected abstract TEarnable Create(TInternalEarnable earnable,
    EarnableLocation[] earnableLocations);
  protected abstract TInternalEarnable Create(TEarnable earnable,
    List<InternalEarnableLocation> earnableLocations);
}
