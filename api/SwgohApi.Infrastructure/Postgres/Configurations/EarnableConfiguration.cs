using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres.Configurations;

public abstract class EarnableConfiguration<T> : IEntityTypeConfiguration<T>
where T : Earnable
{
  public void Configure(EntityTypeBuilder<T> builder)
  {
    builder.ToTable(TableName);
    builder.HasKey(x => x.Id);

    builder.HasIndex(e => e.Name)
      .IsUnique();
  }

  protected abstract string TableName { get; }
}
