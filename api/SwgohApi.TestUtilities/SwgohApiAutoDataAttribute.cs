using AutoFixture;
using AutoFixture.Xunit3;
using SwgohApi.Infrastructure.Models;
using SwgohApi.TestUtilities.Customizations;

namespace SwgohApi.TestUtilities;

public class SwgohApiAutoDataAttribute : AutoDataAttribute
{
  public SwgohApiAutoDataAttribute()
  : base(Customize)
  {
  }

  private static IFixture Customize()
  {
    var fixture = new Fixture();

    fixture.Customize<Marquee>(composer => composer
      .Without(m => m.Character)
      .Without(m => m.Ship));
    fixture.Customize(new DateOnlyCustomization());

    fixture.Customize<Character>(composer => composer
      .Without(c => c.Marquee));
    fixture.Customize<Ship>(composer => composer
      .Without(s => s.Marquee));

    return fixture;
  }
}
