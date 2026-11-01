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

        public async Task<MovieProject> AddProjectAsync(MovieProject project)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            int maxSort = await context.MovieProjects
                .Where(p => p.UserId == project.UserId)
                .Select(p => (int?)p.SortOrder)
                .MaxAsync() ?? 0;

            project.SortOrder = maxSort + 1;

            context.MovieProjects.Add(project);
            await context.SaveChangesAsync();

            return project;
        }

        public async Task<List<MovieProject>> GetProjectsByUserIdAsync(int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.MovieProjects
                .Include(p => p.User)
                .Include(p => p.ProjectShares).ThenInclude(ps => ps.SharedWithUser)
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();
        }

        public async Task<MovieProject?> GetProjectByIdAndUserAsync(int id, int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.MovieProjects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        }

        public async Task<bool> UpdateProjectAsync(MovieProject project, int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var existingProject = await context.MovieProjects
                .FirstOrDefaultAsync(p => p.Id == project.Id && p.UserId == userId);

            if (existingProject == null)
                return false;

            existingProject.Title = project.Title;

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSynopsisAsync(int projectId, int userId, string synopsis)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var project = await context.MovieProjects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
                return false;

            project.Synopsis = synopsis;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProjectAsync(int id, int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var project = await context.MovieProjects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
                return false;

            context.MovieProjects.Remove(project);

            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateGenreAsync(int projectId, int userId, Genre genre)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var project = await context.MovieProjects.FirstOrDefaultAsync(
                p => p.Id == projectId && p.UserId == userId
            );

            if (project == null)
            {
                return false;
            }

            project.Genre = genre;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateLoglineAsync(int projectId, int userId, string logline)
        {
            if (logline.Length > 250)
            {
                return false;
            }

            await using var context = await _contextFactory.CreateDbContextAsync();

            var project = await context.MovieProjects.FirstOrDefaultAsync(
                p => p.Id == projectId && p.UserId == userId
            );

            if (project == null)
            {
                return false;
            }

            project.Logline = logline;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task UpdateProjectsOrderAsync(List<int> orderedIds, int userId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var projects = await context.MovieProjects
                .Where(p => p.UserId == userId)
                .ToListAsync();

            for (int i = 0; i < orderedIds.Count; i++)
            {
                var project = projects.FirstOrDefault(p => p.Id == orderedIds[i]);
                if (project != null)
                {
                    project.SortOrder = i + 1;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
