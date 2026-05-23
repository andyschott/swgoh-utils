using AutoFixture;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities.Customizations;
using InternalCharacter = SwgohApi.Infrastructure.Models.Character;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;
using InternalShip = SwgohApi.Infrastructure.Models.Ship;

namespace SwgohApi.Tests.Mappers;

public class MarqueeDateMapperTests
{
  private readonly MarqueeDateMapper _mapper = new();

  [Theory, AutoDomainData]
  public void MapTo_CharacterMarquee_Successful(InternalCharacter character,
    IFixture fixture)
  {
    var source = fixture.Build<InternalMarquee>()
      .With(m => m.Character, character)
      .With(m => m.CharacterId, character.Id)
      .With(m => m.Ship, (InternalShip?)null)
      .With(m => m.ShipId, (string?)null)
      .Create();

    var result = _mapper.MapTo(source);

    Assert.Equal(character.Name, result.Name);
    Assert.Equal(source.IntroductionDate, result.IntroductionDate);
    Assert.Equal(source.MarqueeEventDate, result.MarqueeEventDate);
    Assert.Equal(source.ShipmentDate, result.ShipmentDate);
    Assert.Equal(source.FarmDate, result.FarmDate);
    Assert.Equal(source.AccelerationDate, result.AccelerationDate);
  }

  [Theory, AutoDomainData]
  public void MapTo_ShipMarquee_Successful(InternalShip ship,
    IFixture fixture)
  {
    var source = fixture.Build<InternalMarquee>()
      .With(m => m.Character, (InternalCharacter?)null)
      .With(m => m.CharacterId, (string?)null)
      .With(m => m.Ship, ship)
      .With(m => m.ShipId, ship.Id)
      .Create();

    var result = _mapper.MapTo(source);

    Assert.Equal(ship.Name, result.Name);
    Assert.Equal(source.IntroductionDate, result.IntroductionDate);
    Assert.Equal(source.MarqueeEventDate, result.MarqueeEventDate);
    Assert.Equal(source.ShipmentDate, result.ShipmentDate);
    Assert.Equal(source.FarmDate, result.FarmDate);
    Assert.Equal(source.AccelerationDate, result.AccelerationDate);
  }

  [Theory, AutoDomainData]
  public void MapTo_NoCharacterOrShip_ThrowsArgumentException(InternalMarquee source)
  {
    source.Character = null;
    source.CharacterId = null;
    source.Ship = null;
    source.ShipId = null;

    Assert.Throws<ArgumentException>(() => _mapper.MapTo(source));
  }

  [Theory, AutoDomainData]
  public void MapFrom_ThrowsNotImplementedException(MarqueeDate source)
  {
    Assert.Throws<NotImplementedException>(() => _mapper.MapFrom(source));
  }

  class AutoDomainDataAttribute : AutoDataAttribute
  {
    public AutoDomainDataAttribute()
      : base(Customize)
    {
    }

    private static IFixture Customize()
    {
      var fixture = new Fixture();

      fixture.Customize(new MarqueeCustomization());

      return fixture;
    }
  }
}
