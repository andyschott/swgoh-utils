using AutoFixture;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Tests.Customizations;

public class MarqueeCustomization : ICustomization
{
  public void Customize(IFixture fixture)
  {
    fixture.Customize<Marquee>(composer => composer
      .Without(m => m.Character)
      .Without(m => m.Ship));
  }
}
