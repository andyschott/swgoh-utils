namespace SwgohApi.Mapping;

public interface IMapper<TSource, TDestination>
{
  TDestination MapTo(TSource source);
  TSource MapFrom(TDestination destination);
}
