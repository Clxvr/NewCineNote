using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineNote.Migrations
{

    public partial class AddSortOrderToMovieProject : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "movie_projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "movie_projects");
        }
    }
}
