using AutoFixture;
using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;
using SwgohApi.Infrastructure.Tests.Customizations;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresCharacterRepositoryTests : AbstractPostgresRepositoryTests
{
  private readonly PostgresCharacterRepository _repository;

  public PostgresCharacterRepositoryTests()
  {
    _repository = new PostgresCharacterRepository(_mockDbContext.Object,
      _mockIdGenerator.Object);
  }

  [Theory, AutoDomainData]
  public async Task CreateCharacter_Successful(string id,
    string name,
    EarnableLocation[] locations,
    bool isAccelerated,
    Marquee marquee)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.Characters);

    var result = await _repository.CreateCharacter(name,
      locations,
      isAccelerated,
      marquee);

    Assert.Equal(id, result.Id);
    Assert.Equal(name, result.Name);
    Assert.Equal(locations, result.Locations);
    Assert.Equal(isAccelerated, result.IsAccelerated);
    Assert.Equal(marquee, result.Marquee);
  }

  [Theory, AutoDomainData]
  public async Task CreateCharacter_NotMarquee_Successful(string id,
    string name,
    EarnableLocation[] locations,
    bool isAccelerated)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.Characters);

    var result = await _repository.CreateCharacter(name,
      locations,
      isAccelerated,
      null);

    Assert.Equal(id, result.Id);
    Assert.Equal(name, result.Name);
    Assert.Equal(locations, result.Locations);
    Assert.Equal(isAccelerated, result.IsAccelerated);
    Assert.Null(result.Marquee);
  }

  [Theory, AutoDomainData]
  public async Task GetCharacterByName_Successful(Character character)
  {
    SetupMockEntities(dbContext => dbContext.Characters, [character]);

    var result = await _repository.GetCharacterByName(character.Name);

    Assert.Same(character, result);
  }

  [Theory, AutoDomainData]
  public async Task GetCharacterByName_NotFound_ReturnsNull(Character character,
    string name)
  {
    SetupMockEntities(dbContext => dbContext.Characters, [character]);

    var result = await _repository.GetCharacterByName(name);

    Assert.Null(result);
  }

  [Theory, AutoDomainData]
  public async Task GetCharacter_Successful(Character character)
  {
    SetupMockEntities(dbContext => dbContext.Characters, [character]);

    var result = await _repository.GetCharacter(character.Id);

    Assert.Same(character, result);
  }

  [Theory, AutoDomainData]
  public async Task GetCharacter_NotFound_ReturnsNull(Character character,
    string id)
  {
    SetupMockEntities(dbContext => dbContext.Characters, [character]);

    var result = await _repository.GetCharacter(id);

    Assert.Null(result);
  }

  [Theory, AutoDomainData]
  public async Task SaveCharacter_Successful(Character character)
  {
    SetupMockEntities(dbContext => dbContext.Characters, [character]);

    var exception = await Record.ExceptionAsync(() => _repository.SaveCharacter(character));

    Assert.Null(exception);
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
      fixture.Customize(new DateOnlyCustomization());


      return fixture;
    }
  }
}
