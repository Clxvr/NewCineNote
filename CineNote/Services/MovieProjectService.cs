
using CineNote.Data;
using CineNote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNote.Services
{
    public class MovieProjectService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public MovieProjectService(
            IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<MovieProject> CreateProjectAsync(MovieProject project)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.MovieProjects.Add(project);
            await context.SaveChangesAsync();

            return project;
        }

        public async Task<List<MovieProject>> GetProjectsByUserIdAsync(int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.MovieProjects
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<MovieProject?> GetProjectByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.MovieProjects
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> UpdateProjectAsync(MovieProject project)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.MovieProjects.Update(project);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var project = await context.MovieProjects.FindAsync(id);

            if (project == null)
                return false;

            context.MovieProjects.Remove(project);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
