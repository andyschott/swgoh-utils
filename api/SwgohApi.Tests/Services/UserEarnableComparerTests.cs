using AutoFixture;
using Microsoft.AspNetCore.Components; using SwgohApi.Models.Earnables;
using SwgohApi.Services;
using SwgohApi.TestUtilities;

namespace SwgohApi.Tests.Services;

public class UserEarnableComparerTests
{
  private readonly UserEarnableComparer _comparer = new();

  [Theory, SwgohApiAutoData]
  public void Compare_SameRefernce_ReturnsZero(Character character)
  {
    var result = _comparer.Compare(character, character);

    Assert.Equal(0, result);
  }

  [Theory, SwgohApiAutoData]
  public void Compare_XIsNull_ReturnOne(Character character)
  {
    var result = _comparer.Compare(null, character);

    Assert.Equal(1, result);
  }

  [Theory, SwgohApiAutoData]
  public void Compare_YIsNull_ReturnMinusOne(Character character)
  {
    var result = _comparer.Compare(character, null);

    Assert.Equal(-1, result);
  }

  [Theory, SwgohApiAutoData]
  public void Compare_XShardsIsNull_Throws(Character character,
    IFixture fixture)
  {
    var first = fixture.Build<Character>()
      .With(ch => ch.Shards, (EarnableShards?)null)
      .Create();

    Assert.Throws<ArgumentException>(() => _comparer.Compare(first, character));
  }

  [Theory, SwgohApiAutoData]
  public void Compare_YShardsIsNull_Throws(Character character,
    IFixture fixture)
  {
    var second = fixture.Build<Character>()
      .With(ch => ch.Shards, (EarnableShards?)null)
      .Create();

    Assert.Throws<ArgumentException>(() => _comparer.Compare(character, second));
  }

  [Theory, SwgohApiAutoData]
  public void Compare_SameFarmingStatus_SortByName(FarmingStatus farmingStatus,
    IFixture fixture)
  {
    var first = fixture.Build<Character>()
      .With(ch => ch.Name, "John")
      .With(ch => ch.Shards, fixture.Build<EarnableShards>()
        .With(s => s.FarmingStatus, farmingStatus)
        .Create())
      .Create();
    var second = fixture.Build<Character>()
      .With(ch => ch.Name, "Jane")
      .With(ch => ch.Shards, fixture.Build<EarnableShards>()
        .With(s => s.FarmingStatus, farmingStatus)
        .Create())
      .Create();

    var result = _comparer.Compare(first, second);

    Assert.True(result > 0);
  }
  [Theory]
  [InlineSwgohApiAutoData(FarmingStatus.Active, FarmingStatus.Backlog, -1)]
  [InlineSwgohApiAutoData(FarmingStatus.Active, FarmingStatus.Done, -1)]
  [InlineSwgohApiAutoData(FarmingStatus.Backlog, FarmingStatus.Active, 1)]
  [InlineSwgohApiAutoData(FarmingStatus.Backlog, FarmingStatus.Done, -1)]
  [InlineSwgohApiAutoData(FarmingStatus.Done, FarmingStatus.Backlog, 1)]
  [InlineSwgohApiAutoData(FarmingStatus.Done, FarmingStatus.Active, 1)]
  public void Compare_StatusTestCases(FarmingStatus firstFarmingStatus,
    FarmingStatus secondFarmingStatus,
    int expectedResult,
    IFixture fixture)
  {
    var first = fixture.Build<Character>()
      .With(ch => ch.Shards, fixture.Build<EarnableShards>()
        .With(s => s.FarmingStatus, firstFarmingStatus)
        .Create())
      .Create();
    var second = fixture.Build<Character>()
      .With(ch => ch.Shards, fixture.Build<EarnableShards>()
        .With(s => s.FarmingStatus, secondFarmingStatus)
        .Create())
      .Create();

    var result = _comparer.Compare(first, second);

    Assert.Equal(expectedResult, result);
  }
}
