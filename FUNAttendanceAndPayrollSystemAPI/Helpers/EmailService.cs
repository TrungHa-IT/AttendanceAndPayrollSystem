using DataTransferObject.EmailDTO;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace FUNAttendanceAndPayrollSystemAPI.Helpers
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_settings.SmtpServer)
            {
                Port = _settings.Port,
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.SenderPassword),
                EnableSsl = _settings.UseSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.DisplayName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
