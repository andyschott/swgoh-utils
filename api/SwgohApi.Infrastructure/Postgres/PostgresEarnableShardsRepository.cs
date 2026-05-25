using Microsoft.EntityFrameworkCore;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresEarnableShardsRepository : IEarnableShardsRepository
{
  private readonly IPostgresDbContext _dbContext;
  private readonly IIdGenerator _idGenerator;

  public PostgresEarnableShardsRepository(IPostgresDbContext dbContext,
    IIdGenerator idGenerator)
  {
    _dbContext = dbContext;
    _idGenerator = idGenerator;
  }

  public async Task<EarnableShards> CreateEarnableShards(User user,
    Earnable earnable,
    int shards,
    FarmingStatus farmingStatus)
  {
    var earnableShards = new EarnableShards
    {
      Id = _idGenerator.CreateId(),
      Shards = shards,
      FarmingStatus = farmingStatus,
      UserId = user.Id,
      User = user
    };

    if (earnable is Character character)
    {
      earnableShards.CharacterId = character.Id;
      earnableShards.Character = character;

      earnableShards.ShipId = null;
      earnableShards.Ship = null;
    }
    else if (earnable is Ship ship)
    {
      earnableShards.ShipId = ship.Id;
      earnableShards.Ship = ship;

      earnableShards.CharacterId = null;
      earnableShards.Character = null;
    }

    await _dbContext.EarnableShards.AddAsync(earnableShards);
    await  _dbContext.SaveChangesAsync();
    return earnableShards;
  }

  public async Task<EarnableShards?> GetEarnableShards(string id)
  {
    return await _dbContext.EarnableShards
      .FirstOrDefaultAsync(e => e.Id == id);
  }

  public async Task SaveEarnableShards(EarnableShards earnableShards)
  {
    _dbContext.EarnableShards.Update(earnableShards);
    await _dbContext.SaveChangesAsync();
  }
}
