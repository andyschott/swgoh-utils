using AutoFixture;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;
using SwgohApi.TestUtilities.Customizations;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresMarqueeRepositoryTests : AbstractPostgresRepositoryTests
{
  private readonly PostgresMarqueeRepository _repository;

  public PostgresMarqueeRepositoryTests()
  {
    _repository = new PostgresMarqueeRepository(_mockDbContext.Object,
      _mockIdGenerator.Object);
  }

  [Theory, AutoDomainData]
  public async Task CreateMarquee_Successful(string id,
    Character character,
    DateOnly introductionDate,
    DateOnly marqueeEventDate,
    DateOnly shipmentDate,
    DateOnly farmDate,
    DateOnly? accelerationDate)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.Marquees);

    var result = await _repository.CreateMarquee(character,
      introductionDate,
      marqueeEventDate,
      shipmentDate,
      farmDate,
      accelerationDate);

    Assert.Equal(id, result.Id);
    Assert.Equal(character.Id, result.CharacterId);
    Assert.Same(character, result.Character);
    Assert.Null(result.ShipId);
    Assert.Null(result.Ship);
    Assert.Equal(introductionDate, result.IntroductionDate);
    Assert.Equal(marqueeEventDate, result.MarqueeEventDate);
    Assert.Equal(shipmentDate, result.ShipmentDate);
    Assert.Equal(farmDate, result.FarmDate);
    Assert.Equal(accelerationDate, result.AccelerationDate);

    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
  }

  [Theory, AutoDomainData]
  public async Task CreateMarquee_Ship_Successful(string id,
    Ship ship,
    DateOnly introductionDate,
    DateOnly marqueeEventDate,
    DateOnly shipmentDate,
    DateOnly farmDate,
    DateOnly? accelerationDate)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.Marquees);

    var result = await _repository.CreateMarquee(ship,
      introductionDate,
      marqueeEventDate,
      shipmentDate,
      farmDate,
      accelerationDate);

    Assert.Equal(id, result.Id);
    Assert.Equal(ship.Id, result.ShipId);
    Assert.Same(ship, result.Ship);
    Assert.Null(result.CharacterId);
    Assert.Null(result.Character);
    Assert.Equal(introductionDate, result.IntroductionDate);
    Assert.Equal(marqueeEventDate, result.MarqueeEventDate);
    Assert.Equal(shipmentDate, result.ShipmentDate);
    Assert.Equal(farmDate, result.FarmDate);
    Assert.Equal(accelerationDate, result.AccelerationDate);

    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
  }

  [Theory, AutoDomainData]
  public async Task GetMarquee_Successful(Marquee marquee)
  {
    SetupMockEntities(dbContext => dbContext.Marquees, [marquee]);

    var result = await _repository.GetMarquee(marquee.Id);

    Assert.Same(marquee, result);
  }

  [Theory, AutoDomainData]
  public async Task GetMarquee_NotFound_ReturnsNull(Marquee marquee,
    string id)
  {
    SetupMockEntities(dbContext => dbContext.Marquees, [marquee]);

    var result = await _repository.GetMarquee(id);

    Assert.Null(result);
  }

  [Theory, AutoDomainData]
  public async Task SaveMarquee_Successful(Marquee marquee)
  {
    var mockMarqueesDbSet = CreateMockDbSet(dbContext => dbContext.Marquees);

    var exception = await Record.ExceptionAsync(() => _repository.SaveMarquee(marquee));

    Assert.Null(exception);
    mockMarqueesDbSet.Verify(dbSet => dbSet.Update(marquee), Times.Once);
    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
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
