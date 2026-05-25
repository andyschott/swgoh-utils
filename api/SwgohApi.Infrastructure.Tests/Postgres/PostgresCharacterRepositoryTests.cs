using SwgohApi.Infrastructure.Models;
using SwgohApi.Infrastructure.Postgres;
using SwgohApi.TestUtilities;

namespace SwgohApi.Infrastructure.Tests.Postgres;

public class PostgresCharacterRepositoryTests : AbstractPostgresRepositoryTests
{
  private readonly PostgresCharacterRepository _repository;

  public PostgresCharacterRepositoryTests()
  {
    _repository = new PostgresCharacterRepository(_mockDbContext.Object,
      _mockIdGenerator.Object);
  }

  [Theory, SwgohApiAutoData]
  public async Task CreateCharacter_Successful(string id,
    string name,
    EarnableLocation[] locations,
    bool isAccelerated)
  {
    _mockIdGenerator.Setup(idGenerator => idGenerator.CreateId())
      .Returns(id);

    CreateMockDbSet(dbContext => dbContext.Characters);

    var result = await _repository.CreateCharacter(name,
      locations,
      isAccelerated);

    Assert.Equal(id, result.Id);
    Assert.Equal(name, result.Name);
    Assert.Equal(locations, result.Locations);
    Assert.Equal(isAccelerated, result.IsAccelerated);
  }


  [Theory, SwgohApiAutoData]
  public async Task GetCharacter_Successful(Character character)
  {
    SetupMockEntities(dbContext => dbContext.Characters, [character]);

    var result = await _repository.GetCharacter(character.Id);

    Assert.Same(character, result);
  }

  [Theory, SwgohApiAutoData]
  public async Task GetCharacter_NotFound_ReturnsNull(Character character,
    string id)
  {
    SetupMockEntities(dbContext => dbContext.Characters, [character]);

    var result = await _repository.GetCharacter(id);

    Assert.Null(result);
  }

  [Theory, SwgohApiAutoData]
  public async Task SaveCharacter_Successful(Character character)
  {
    SetupMockEntities(dbContext => dbContext.Characters, [character]);

    var exception = await Record.ExceptionAsync(() => _repository.SaveCharacter(character));

    Assert.Null(exception);
    _mockDbContext.Verify(dbContext => dbContext.SaveChangesAsync(), Times.Once);
  }
}
