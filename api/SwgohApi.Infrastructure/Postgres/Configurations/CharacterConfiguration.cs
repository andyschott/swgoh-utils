using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres.Configurations;

public class CharacterConfiguration : EarnableConfiguration<Character>
{
  protected override string TableName => "Characters";
}
