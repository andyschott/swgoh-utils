using System.ComponentModel.DataAnnotations;

namespace SwgohApi.Infrastructure.Postgres;

public class PostgresConfiguration
{
  [Required(AllowEmptyStrings = false)]
  public required string ConnectionString { get; set; }
}
