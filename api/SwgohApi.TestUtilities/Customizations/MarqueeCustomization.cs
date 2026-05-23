using AutoFixture;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.TestUtilities.Customizations;

public class MarqueeCustomization : ICustomization
{
  public void Customize(IFixture fixture)
  {
    fixture.Customize<Marquee>(composer => composer
      .Without(m => m.Character)
      .Without(m => m.Ship));
    fixture.Customize(new DateOnlyCustomization());
  }
}
