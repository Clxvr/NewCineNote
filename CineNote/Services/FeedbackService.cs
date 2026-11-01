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
            using (var context = await _contextFactory.CreateDbContextAsync())
            {
                feedback.CreatedAt = DateTime.UtcNow;
                context.ContactFeedbacks.Add(feedback);
                await context.SaveChangesAsync();
            }

            try
            {
                var mskTime = DateTime.UtcNow.AddHours(3);
                string subject = $"[CineNote Feedback] {feedback.Subject} от {feedback.Name}";
                string htmlMessage = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 25px; border: 1px solid #e5e7eb; border-radius: 12px; background-color: #ffffff;'>
                        <div style='border-bottom: 2px solid #F59E0B; padding-bottom: 15px; margin-bottom: 20px;'>
                            <h2 style='color: #111827; margin: 0; font-size: 20px;'>🎬 CineNote — Новое обращение в поддержку</h2>
                        </div>
                        <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px; font-size: 14px;'>
                            <tr>
                                <td style='padding: 8px 0; color: #6b7280; width: 140px;'>Отправитель:</td>
                                <td style='padding: 8px 0; color: #111827; font-weight: 600;'>{feedback.Name} (&lt;{feedback.Email}&gt;)</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px 0; color: #6b7280;'>Тема:</td>
                                <td style='padding: 8px 0; color: #111827; font-weight: 600;'>{feedback.Subject}</td>
                            </tr>
                            <tr>
                                <td style='padding: 8px 0; color: #6b7280;'>Дата и время:</td>
                                <td style='padding: 8px 0; color: #111827;'>{mskTime:dd.MM.yyyy HH:mm} (МСК)</td>
                            </tr>
                        </table>
                        <div style='background-color: #f9fafb; border: 1px solid #e5e7eb; padding: 18px; border-radius: 8px;'>
                            <span style='display: block; font-size: 13px; font-weight: 700; color: #374151; margin-bottom: 10px; text-transform: uppercase; letter-spacing: 0.5px;'>Текст сообщения:</span>
                            <p style='white-space: pre-wrap; margin: 0; color: #1f2937; line-height: 1.6; font-size: 15px;'>{feedback.Message}</p>
                        </div>
                        <div style='margin-top: 25px; pt-3; border-top: 1px solid #f3f4f6; text-align: center; color: #9ca3af; font-size: 12px;'>
                            CineNote Support System &bull; Сообщение сгенерировано автоматически
                        </div>
                    </div>";

                await _emailService.SendEmailAsync("support@cinenote.com", subject, htmlMessage);
            }
            catch
            {
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
