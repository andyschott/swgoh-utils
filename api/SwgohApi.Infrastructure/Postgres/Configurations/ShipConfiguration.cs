using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres.Configurations;

public class ShipConfiguration : EarnableConfiguration<Ship>
{
  protected override string TableName => "Ships";
}
