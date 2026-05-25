using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres.Configurations;

public class EarnableShardsConfiguration : IEntityTypeConfiguration<EarnableShards>
{
  public void Configure(EntityTypeBuilder<EarnableShards> builder)
  {
    builder.ToTable("EarnableShards");
    builder.HasKey(x => x.Id);

    // User relationship
    builder.HasOne(e => e.User)
      .WithMany(u => u.EarnableShards)
      .HasForeignKey(u => u.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    // Character relationship
    builder.HasOne(e => e.Character)
      .WithOne(c => c.EarnableShards)
      .HasForeignKey<EarnableShards>(e => e.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);

    //Ship relationship
    builder.HasOne(e => e.Ship)
      .WithOne(s => s.EarnableShards)
      .HasForeignKey<EarnableShards>(e => e.ShipId)
      .OnDelete(DeleteBehavior.Cascade);

    // Unique constraints
    builder.ToTable(table =>
    {
      table.HasCheckConstraint("CK_Marquee_Entity",
        """
        (
            "CharacterId" IS NOT NULL
            AND
            "ShipId" IS NULL
        )
        OR
        (
            "CharacterId" IS NULL
            AND
            "ShipId" IS NOT NULL
        )
        """);
    });
  }
}
