using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwgohApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EarnableShards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EarnableShards",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CharacterId = table.Column<string>(type: "text", nullable: true),
                    ShipId = table.Column<string>(type: "text", nullable: true),
                    Shards = table.Column<int>(type: "integer", nullable: false),
                    FarmingStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarnableShards", x => x.Id);
                    table.CheckConstraint("CK_Marquee_Entity", "(\n    \"CharacterId\" IS NOT NULL\n    AND\n    \"ShipId\" IS NULL\n)\nOR\n(\n    \"CharacterId\" IS NULL\n    AND\n    \"ShipId\" IS NOT NULL\n)");
                    table.ForeignKey(
                        name: "FK_EarnableShards_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EarnableShards_Ships_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Ships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EarnableShards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EarnableShards_CharacterId",
                table: "EarnableShards",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_EarnableShards_ShipId",
                table: "EarnableShards",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_EarnableShards_UserId",
                table: "EarnableShards",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EarnableShards");
        }
    }
}
