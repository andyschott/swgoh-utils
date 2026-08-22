using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwgohApi.Infrastructure.Models;

namespace SwgohApi.Infrastructure.Postgres.Configurations;

public class ConquestRewardEntityConfiguration : IEntityTypeConfiguration<ConquestReward>
{
  public void Configure(EntityTypeBuilder<ConquestReward> builder)
  {
    builder.ToTable("ConquestRewards");

    builder.HasKey(cr => cr.Id);

    // Character relationship
    builder.HasOne(cr => cr.Character)
      .WithOne(c => c.ConquestReward)
      .HasForeignKey<ConquestReward>(cr => cr.CharacterId)
      .OnDelete(DeleteBehavior.Cascade);

    //Ship relationship
    builder.HasOne(cr => cr.Ship)
      .WithOne(s => s.ConquestReward)
      .HasForeignKey<ConquestReward>(cr => cr.ShipId)
      .OnDelete(DeleteBehavior.Cascade);

    // Unique constraints
    builder.ToTable(table =>
    {
      table.HasCheckConstraint("CK_ConquestReward_Entity",
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
