using SwgohApi.Mapping;

namespace SwgohApi.Tests.Mappers;

public abstract class AbstractEnumMapperTests<TSource, TDestination, TMapper>
where TSource: struct, Enum
where TDestination: struct, Enum
where TMapper: IMapper<TSource, TDestination>, new()
{
  public static TheoryData<TSource, TDestination> Data { get; } = [];

  private readonly TMapper _mapper = new();

  static AbstractEnumMapperTests()
  {
    var sources = Enum.GetValues<TSource>()
      .OrderBy(value => value.ToString());
    var destinations = Enum.GetValues<TDestination>()
      .OrderBy(value => value.ToString());

    foreach (var (source, destination) in sources.Zip(destinations))
    {
      Data.Add(source, destination);
    }
  }

  [Theory, MemberData(nameof(Data))]
  public void MapToTestCases(TSource source, TDestination destination)
  {
    var result = _mapper.MapTo(source);

    Assert.Equal(result, destination);
  }

  [Theory, MemberData(nameof(Data))]
  public void MapFromTestCases(TSource destination, TDestination source)
  {
    var result = _mapper.MapFrom(source);

    Assert.Equal(result, destination);
  }
}
