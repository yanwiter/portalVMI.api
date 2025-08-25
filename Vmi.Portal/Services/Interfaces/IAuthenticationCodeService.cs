using System;
using System.Threading.Tasks;

namespace Vmi.Portal.Data.Repositories.Interfaces
{
    public interface IAuthenticationCodeService
    {
        Task SaveAuthCode(Guid userId, string code, DateTime expiration);
        Task<bool> ValidateCode(Guid userId, string code);
        Task ResendCode(Guid userId);
        Task SendAuthenticationCode(string email, string name, string code);
        Task CleanExpiredCodes();
    }
}