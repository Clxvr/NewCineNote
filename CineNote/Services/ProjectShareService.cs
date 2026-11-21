using CineNote.Data;
using CineNote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNote.Services
{
    public class ProjectShareService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public ProjectShareService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ProjectShare>> GetProjectSharesAsync(int projectId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.ProjectShares
                .Include(ps => ps.SharedWithUser)
                .Where(ps => ps.MovieProjectId == projectId)
                .OrderBy(ps => ps.Id)
                .ToListAsync();
        }

        public async Task<string?> GetUserAccessLevelAsync(int projectId, int userId)
        {
            if (userId <= 0) return null;

            await using var context = await _contextFactory.CreateDbContextAsync();

            var project = await context.MovieProjects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return null;

            if (project.UserId == userId)
            {
                return ProjectAccessLevel.Owner;
            }

            var share = await context.ProjectShares
                .AsNoTracking()
                .FirstOrDefaultAsync(ps => ps.MovieProjectId == projectId && ps.SharedWithUserId == userId);

            return share?.AccessLevel;
        }

        public async Task<bool> CanUserEditProjectAsync(int projectId, int userId)
        {
            var level = await GetUserAccessLevelAsync(projectId, userId);
            return level == ProjectAccessLevel.Owner || level == ProjectAccessLevel.Editor;
        }

        public async Task<bool> CanUserViewProjectAsync(int projectId, int userId)
        {
            var level = await GetUserAccessLevelAsync(projectId, userId);
            return level != null;
        }

        public async Task<bool> IsProjectOwnerAsync(int projectId, int userId)
        {
            if (userId <= 0) return false;

            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.MovieProjects.AnyAsync(p => p.Id == projectId && p.UserId == userId);
        }

        public async Task<(bool Success, string Message, ProjectShare? Share)> InviteCollaboratorByEmailAsync(
            int projectId, int currentUserId, string email, string accessLevel)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, "Укажите Email соавтора.", null);
            }

            string normalizedEmail = email.Trim().ToLower();

            await using var context = await _contextFactory.CreateDbContextAsync();

            var project = await context.MovieProjects
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return (false, "Проект не найден.", null);
            }

            if (project.UserId != currentUserId)
            {
                return (false, "Приглашать соавторов может только создатель (владелец) проекта.", null);
            }

            var targetUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (targetUser == null)
            {
                return (false, $"Пользователь с адресом «{email}» не зарегистрирован в CineNote. Попросите его сначала создать аккаунт.", null);
            }

            if (targetUser.Id == project.UserId)
            {
                return (false, "Вы являетесь владельцем этого проекта. Добавить самого себя в соавторы нельзя.", null);
            }

            var existingShare = await context.ProjectShares
                .FirstOrDefaultAsync(ps => ps.MovieProjectId == projectId && ps.SharedWithUserId == targetUser.Id);

            if (existingShare != null)
            {
                return (false, $"Пользователь «{targetUser.Name}» ({targetUser.Email}) уже добавлен в соавторы (текущая роль: {existingShare.AccessLevel}).", null);
            }

            if (accessLevel != ProjectAccessLevel.Editor && accessLevel != ProjectAccessLevel.Viewer)
            {
                accessLevel = ProjectAccessLevel.Viewer;
            }

            var newShare = new ProjectShare
            {
                MovieProjectId = projectId,
                SharedWithUserId = targetUser.Id,
                AccessLevel = accessLevel
            };

            context.ProjectShares.Add(newShare);
            await context.SaveChangesAsync();

            newShare.SharedWithUser = targetUser;

            return (true, $"Соавтор «{targetUser.Name}» успешно добавлен с правами «{accessLevel}».", newShare);
        }

        public async Task<(bool Success, string Message)> UpdateShareAccessLevelAsync(
            int shareId, int currentUserId, string newAccessLevel)
        {
            if (newAccessLevel != ProjectAccessLevel.Editor && newAccessLevel != ProjectAccessLevel.Viewer)
            {
                return (false, "Недопустимый уровень доступа.");
            }

            await using var context = await _contextFactory.CreateDbContextAsync();

            var share = await context.ProjectShares
                .Include(ps => ps.MovieProject)
                .Include(ps => ps.SharedWithUser)
                .FirstOrDefaultAsync(ps => ps.Id == shareId);

            if (share == null || share.MovieProject == null)
            {
                return (false, "Запись совместного доступа не найдена.");
            }

            if (share.MovieProject.UserId != currentUserId)
            {
                return (false, "Изменять права участников может только владелец проекта.");
            }

            share.AccessLevel = newAccessLevel;
            await context.SaveChangesAsync();

            return (true, $"Права пользователя «{share.SharedWithUser?.Name ?? "соавтора"}» изменены на «{newAccessLevel}».");
        }

        public async Task<(bool Success, string Message)> RemoveShareAsync(int shareId, int currentUserId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var share = await context.ProjectShares
                .Include(ps => ps.MovieProject)
                .Include(ps => ps.SharedWithUser)
                .FirstOrDefaultAsync(ps => ps.Id == shareId);

            if (share == null || share.MovieProject == null)
            {
                return (false, "Запись совместного доступа не найдена.");
            }

            bool isOwner = share.MovieProject.UserId == currentUserId;
            bool isSelf = share.SharedWithUserId == currentUserId;

            if (!isOwner && !isSelf)
            {
                return (false, "У вас нет прав для выполнения этой операции.");
            }

            string userName = share.SharedWithUser?.Name ?? "соавтора";
            context.ProjectShares.Remove(share);
            await context.SaveChangesAsync();

            if (isSelf)
            {
                return (true, "Вы успешно покинули проект совместной работы.");
            }

            return (true, $"Доступ для пользователя «{userName}» успешно отозван.");
        }

        public async Task<(bool Success, string Message)> LeaveProjectAsync(int projectId, int currentUserId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var share = await context.ProjectShares
                .FirstOrDefaultAsync(ps => ps.MovieProjectId == projectId && ps.SharedWithUserId == currentUserId);

            if (share == null)
            {
                return (false, "Вы не являетесь соавтором этого проекта.");
            }

            context.ProjectShares.Remove(share);
            await context.SaveChangesAsync();

            return (true, "Вы успешно покинули проект.");
        }

        public async Task<List<MovieProject>> GetSharedProjectsForUserAsync(int userId)
        {
            if (userId <= 0) return new List<MovieProject>();

            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.ProjectShares
                .Include(ps => ps.MovieProject)
                    .ThenInclude(p => p!.User)
                .Include(ps => ps.MovieProject)
                    .ThenInclude(p => p!.ProjectShares)
                        .ThenInclude(s => s.SharedWithUser)
                .Where(ps => ps.SharedWithUserId == userId && ps.MovieProject != null)
                .Select(ps => ps.MovieProject!)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
    }
}
