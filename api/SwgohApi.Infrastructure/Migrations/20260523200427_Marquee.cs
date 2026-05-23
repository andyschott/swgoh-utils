using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwgohApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Marquee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Marquees",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: true),
                    ShipId = table.Column<string>(type: "text", nullable: true),
                    IntroductionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MarqueeEventDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ShipmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FarmDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AccelerationDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marquees", x => x.Id);
                    table.CheckConstraint("CK_Marquee_Acceleration", "(\n    \"ShipId\" IS NULL\n)\nOR\n(\n    \"AccelerationDate\" IS NULL\n)");
                    table.CheckConstraint("CK_Marquee_Entity", "(\n    \"CharacterId\" IS NOT NULL\n    AND\n    \"ShipId\" IS NULL\n)\nOR\n(\n    \"CharacterId\" IS NULL\n    AND\n    \"ShipId\" IS NOT NULL\n)");
                    table.ForeignKey(
                        name: "FK_Marquees_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Marquees_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Marquees_CharacterId",
                table: "Marquees",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Marquees_ShipId",
                table: "Marquees",
                column: "ShipId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Marquees");
        }
    }
}
