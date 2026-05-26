using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameSense.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LevelConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LevelId = table.Column<string>(type: "TEXT", nullable: false),
                    GameName = table.Column<string>(type: "TEXT", nullable: true),
                    ExpectedDifficulty = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LevelZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LevelConfigId = table.Column<int>(type: "INTEGER", nullable: false),
                    ZoneName = table.Column<string>(type: "TEXT", nullable: false),
                    ZoneDescription = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelZones_LevelConfigs_LevelConfigId",
                        column: x => x.LevelConfigId,
                        principalTable: "LevelConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LevelZones_LevelConfigId",
                table: "LevelZones",
                column: "LevelConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LevelZones");

            migrationBuilder.DropTable(
                name: "LevelConfigs");
        }
    }
}
