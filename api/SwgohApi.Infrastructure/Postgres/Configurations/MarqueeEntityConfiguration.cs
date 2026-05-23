using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres.Configurations;

public class MarqueeEntityConfiguration : IEntityTypeConfiguration<Marquee>
{
  public void Configure(EntityTypeBuilder<Marquee> builder)
  {
    builder.ToTable("Marquees");

    builder.HasKey(m => m.Id);

    // Character relationship
    builder.HasOne(m => m.Character)
      .WithOne(c => c.Marquee)
      .HasForeignKey<Marquee>(m => m.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);

    //Ship relationship
    builder.HasOne(m => m.Ship)
      .WithOne(s => s.Marquee)
      .HasForeignKey<Marquee>(m => m.ShipId)
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

      table.HasCheckConstraint("CK_Marquee_Acceleration",
        """
        (
            "ShipId" IS NULL
        )
        OR
        (
            "AccelerationDate" IS NULL
        )
        """);
    });
  }
}
