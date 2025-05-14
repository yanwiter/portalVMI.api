using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Vmi.Portal.Services.Interfaces;

namespace Vmi.Portal.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool? isHtml)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            if (smtpSettings == null) throw new ArgumentNullException("SMTP settings not configured");

            var host = smtpSettings["Host"] ?? throw new ArgumentNullException("SMTP Host not configured");

            using var client = new SmtpClient(host)
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"])
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            try
            {
                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}