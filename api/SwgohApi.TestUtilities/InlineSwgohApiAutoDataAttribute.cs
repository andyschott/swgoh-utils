using AutoFixture.Xunit3;

namespace SwgohApi.TestUtilities;

public class InlineSwgohApiAutoDataAttribute : InlineAutoDataAttribute
{
  public InlineSwgohApiAutoDataAttribute(params object[] values)
  : base(SwgohApiAutoDataAttribute.Customize, values)
  {
  }
}
