using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineNote.Migrations
{
    /// <inheritdoc />
    public partial class AddSortOrderToMovieProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "movie_projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "movie_projects");
        }
    }
}
