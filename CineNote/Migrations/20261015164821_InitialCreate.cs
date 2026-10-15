using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineNote.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    global_role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movie_projects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    genre = table.Column<int>(type: "int", nullable: false),
                    logline = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    synopsis = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_projects", x => x.id);
                    table.ForeignKey(
                        name: "FK_movie_projects_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_project_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.id);
                    table.ForeignKey(
                        name: "FK_characters_movie_projects_movie_project_id",
                        column: x => x.movie_project_id,
                        principalTable: "movie_projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_shares",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_project_id = table.Column<int>(type: "int", nullable: false),
                    shared_with_user_id = table.Column<int>(type: "int", nullable: false),
                    access_level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_shares", x => x.id);
                    table.ForeignKey(
                        name: "FK_project_shares_movie_projects_movie_project_id",
                        column: x => x.movie_project_id,
                        principalTable: "movie_projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_shares_users_shared_with_user_id",
                        column: x => x.shared_with_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "scenes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    movie_project_id = table.Column<int>(type: "int", nullable: false),
                    scene_number = table.Column<int>(type: "int", nullable: false),
                    location_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    location_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    time_of_day = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    action_description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scenes", x => x.id);
                    table.ForeignKey(
                        name: "FK_scenes_movie_projects_movie_project_id",
                        column: x => x.movie_project_id,
                        principalTable: "movie_projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scene_characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    scene_id = table.Column<int>(type: "int", nullable: false),
                    character_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scene_characters", x => x.id);
                    table.ForeignKey(
                        name: "FK_scene_characters_characters_character_id",
                        column: x => x.character_id,
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_scene_characters_scenes_scene_id",
                        column: x => x.scene_id,
                        principalTable: "scenes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_characters_movie_project_id",
                table: "characters",
                column: "movie_project_id");

            migrationBuilder.CreateIndex(
                name: "IX_movie_projects_user_id",
                table: "movie_projects",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_shares_movie_project_id",
                table: "project_shares",
                column: "movie_project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_shares_shared_with_user_id",
                table: "project_shares",
                column: "shared_with_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_scene_characters_character_id",
                table: "scene_characters",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "IX_scene_characters_scene_id",
                table: "scene_characters",
                column: "scene_id");

            migrationBuilder.CreateIndex(
                name: "IX_scenes_movie_project_id",
                table: "scenes",
                column: "movie_project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "project_shares");

            migrationBuilder.DropTable(
                name: "scene_characters");

            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "scenes");

            migrationBuilder.DropTable(
                name: "movie_projects");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
