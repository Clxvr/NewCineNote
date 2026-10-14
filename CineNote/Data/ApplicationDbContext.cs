using CineNote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNote.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MovieProject> MovieProjects { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Scene> Scenes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProjectShare> ProjectShares { get; set; }
        public DbSet<SceneCharacter> SceneCharacters { get; set; }
    }
}