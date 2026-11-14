using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineNote.Migrations
{
    /// <inheritdoc />
    public partial class AddSceneStatusAndDramaticStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "dramatic_structure",
                table: "scenes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "scenes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dramatic_structure",
                table: "scenes");

            migrationBuilder.DropColumn(
                name: "status",
                table: "scenes");
        }
    }
}
