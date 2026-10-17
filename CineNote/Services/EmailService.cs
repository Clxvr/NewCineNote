using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace CineNote.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var emailMessage = new MimeMessage();

        
            emailMessage.From.Add(new MailboxAddress(
                emailSettings["SenderName"] ?? "CineNote Support",
                emailSettings["SenderEmail"] ?? "noreply@cinenote.com"));

            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(
                    emailSettings["Server"] ?? "sandbox.smtp.mailtrap.io",
                    int.Parse(emailSettings["Port"] ?? "2525"),
                    false);

                await client.AuthenticateAsync(
                    emailSettings["Username"] ?? "",
                    emailSettings["Password"] ?? "");

                await client.SendAsync(emailMessage);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}