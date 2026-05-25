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

    fixture.Customize<EarnableShards>(composer => composer
      .Without(e => e.Character)
      .Without(e => e.Ship));

    fixture.Customize<Character>(composer => composer
      .Without(c => c.Marquee)
      .Without(c => c.EarnableShards));
    fixture.Customize<Ship>(composer => composer
      .Without(s => s.Marquee)
      .Without(s => s.EarnableShards));
    fixture.Customize<User>(composer => composer
      .With(u => u.EarnableShards, []));


    return fixture;
  }
}
