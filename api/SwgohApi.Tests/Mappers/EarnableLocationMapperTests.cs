using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using InternalEarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;

namespace SwgohApi.Tests.Mappers;

public class EarnableLocationMapperTests
{
  public static TheoryData<InternalEarnableLocation, EarnableLocation> Data { get; } = [];

  private readonly EarnableLocationMapper _mapper = new();

  static EarnableLocationMapperTests()
  {
    var sources = Enum.GetValues<InternalEarnableLocation>()
      .OrderBy(value => value.ToString());
    var destinations = Enum.GetValues<EarnableLocation>()
      .OrderBy(value => value.ToString());

    foreach (var (source, destination) in sources.Zip(destinations))
    {
      Data.Add(source, destination);
    }
  }

  [Theory, MemberData(nameof(Data))]
  public void MapToTestCases(InternalEarnableLocation source, EarnableLocation destination)
  {
    var result = _mapper.MapTo(source);

    Assert.Equal(result, destination);
  }

  [Theory, MemberData(nameof(Data))]
  public void MapFromTestCases(InternalEarnableLocation destination, EarnableLocation source)
  {
    var result = _mapper.MapFrom(source);

    Assert.Equal(result, destination);
  }
}
