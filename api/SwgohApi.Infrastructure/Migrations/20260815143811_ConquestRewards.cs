using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwgohApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConquestRewards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConquestRewards",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: true),
                    ShipId = table.Column<string>(type: "text", nullable: true),
                    RewardPhase = table.Column<int>(type: "integer", nullable: false),
                    InitialUnlockDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FinalRewardCreateDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProvingGroundsDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConquestRewards", x => x.Id);
                    table.CheckConstraint("CK_ConquestReward_Entity", "(\n    \"CharacterId\" IS NOT NULL\n    AND\n    \"ShipId\" IS NULL\n)\nOR\n(\n    \"CharacterId\" IS NULL\n    AND\n    \"ShipId\" IS NOT NULL\n)");
                    table.ForeignKey(
                        name: "FK_ConquestRewards_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConquestRewards_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConquestRewards_CharacterId",
                table: "ConquestRewards",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConquestRewards_ShipId",
                table: "ConquestRewards",
                column: "ShipId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConquestRewards");
        }
    }
}
