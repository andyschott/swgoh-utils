using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.ToTable("Users");
    builder.HasKey(user => user.Id);
    builder.HasIndex(user => user.Email)
      .IsUnique();
  }
}
