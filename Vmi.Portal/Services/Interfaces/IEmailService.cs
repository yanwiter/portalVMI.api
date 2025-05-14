using System.Threading.Tasks;

namespace Vmi.Portal.Services.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool? isHtml);
}