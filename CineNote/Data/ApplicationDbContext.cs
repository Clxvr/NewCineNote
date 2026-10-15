using Microsoft.EntityFrameworkCore;
using CineNote.Models;

namespace CineNote.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MovieProject> MovieProjects { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Scene> Scenes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProjectShare> ProjectShares { get; set; }
        public DbSet<SceneCharacter> SceneCharacters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Защита от циклов и множественных каскадных путей для шаринга проектов
            modelBuilder.Entity<ProjectShare>()
                .HasOne(ps => ps.SharedWithUser)
                .WithMany(u => u.ProjectShares)
                .HasForeignKey(ps => ps.SharedWithUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Защита от циклов для связей сцен и персонажей в SceneCharacter
            modelBuilder.Entity<SceneCharacter>()
                .HasOne(sc => sc.Scene)
                .WithMany(s => s.SceneCharacters)
                .HasForeignKey(sc => sc.SceneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SceneCharacter>()
                .HasOne(sc => sc.Character)
                .WithMany(c => c.SceneCharacters)
                .HasForeignKey(sc => sc.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}