using CineNote.Data;
using CineNote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNote.Services
{
    public class FeedbackService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly EmailService _emailService;

        public FeedbackService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            EmailService emailService)
        {
            _contextFactory = contextFactory;
            _emailService = emailService;
        }

        public async Task<bool> SendFeedbackAsync(ContactFeedback feedback)
        {
            // 1. Сохранение в базу данных
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                feedback.CreatedAt = DateTime.UtcNow;
                context.ContactFeedbacks.Add(feedback);
                await context.SaveChangesAsync();
            }

            // 2. Отправка уведомления на почту (в Mailtrap)
            try
            {
                string subject = $"[CineNote Feedback] {feedback.Subject} от {feedback.Name}";
                string htmlMessage = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                        <h2 style='color: #1a1a1a; margin-top: 0;'>Новое обращение в поддержку CineNote</h2>
                        <hr style='border: none; border-top: 1px solid #ddd; margin: 15px 0;' />
                        <p><strong>От кого:</strong> {feedback.Name} (&lt;{feedback.Email}&gt;)</p>
                        <p><strong>Тема:</strong> {feedback.Subject}</p>
                        <p><strong>Дата:</strong> {feedback.CreatedAt:dd.MM.yyyy HH:mm} (UTC)</p>
                        <div style='background-color: #f9f9f9; padding: 15px; border-radius: 6px; margin-top: 15px;'>
                            <strong style='display: block; margin-bottom: 8px;'>Текст сообщения:</strong>
                            <p style='white-space: pre-wrap; margin: 0; color: #333;'>{feedback.Message}</p>
                        </div>
                    </div>";

                await _emailService.SendEmailAsync("support@cinenote.com", subject, htmlMessage);
            }
            catch
            {
                // Если почтовый сервер временно недоступен, сообщение в БД уже сохранено
            }

            return true;
        }

        public async Task<List<ContactFeedback>> GetAllFeedbacksAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.ContactFeedbacks
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }
    }
}
