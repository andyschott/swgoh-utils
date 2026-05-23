using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
  public void Configure(EntityTypeBuilder<RefreshToken> builder)
  {
    builder.ToTable("RefreshTokens");
    builder.HasKey(token => token.Id);

    builder.HasIndex(token => token.TokenHash)
      .IsUnique();

    builder.HasIndex(token => new { token.UserId, token.RevokedAtUtc, token.ExpiresAtUtc });
  }
}
