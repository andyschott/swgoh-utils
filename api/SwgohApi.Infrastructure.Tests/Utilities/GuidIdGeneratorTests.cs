using SwgohApi.Infrastructure.Utilities;

namespace SwgohApi.Infrastructure.Tests.Utilities;

public class GuidIdGeneratorTests
{
  private readonly GuidIdGenerator _guidIdGenerator = new();

  [Fact]
  public void CreateId_IsGuid()
  {
    var id = _guidIdGenerator.CreateId();
    Assert.True(Guid.TryParse(id, out _));
  }
}
