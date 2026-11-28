using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineNote.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterArcSteps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_arc_steps",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    character_id = table.Column<int>(type: "int", nullable: false),
                    scene_id = table.Column<int>(type: "int", nullable: true),
                    act = table.Column<int>(type: "int", nullable: false),
                    stage_title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    emotional_note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_arc_steps", x => x.id);
                    table.ForeignKey(
                        name: "FK_character_arc_steps_characters_character_id",
                        column: x => x.character_id,
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_arc_steps_scenes_scene_id",
                        column: x => x.scene_id,
                        principalTable: "scenes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_character_arc_steps_character_id",
                table: "character_arc_steps",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "IX_character_arc_steps_scene_id",
                table: "character_arc_steps",
                column: "scene_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_arc_steps");
        }
    }
}
