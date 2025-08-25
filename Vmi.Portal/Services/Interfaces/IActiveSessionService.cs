using System.Collections.Generic;
using System.Threading.Tasks;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Services.Interfaces;

public interface IActiveSessionService
{
    Task AddSession(ActiveSession session);
    Task CreateSession(Guid userId, string refreshToken, string ipAddress, string deviceInfo);
    Task RemoveSession(string userId, string refreshToken);
    Task<bool> IsActiveSession(string userId, string refreshToken);
    Task<IEnumerable<ActiveSession>> GetUserSessions(string userId);
    Task RevokeAllSessions(string userId);
    Task<ActiveSession> GetSessionByRefreshToken(string refreshToken);
}