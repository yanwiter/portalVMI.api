using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vmi.Portal.Entities;
using Vmi.Portal.Services.Interfaces;
using System;

namespace Vmi.Portal.Services;

public class ActiveSessionService : IActiveSessionService
{
    private readonly ConcurrentDictionary<string, List<ActiveSession>> _activeSessions = new();

    public Task AddSession(ActiveSession session)
    {
        _activeSessions.AddOrUpdate(session.UserId,
            new List<ActiveSession> { session },
            (key, existingList) =>
            {
                existingList.Add(session);
                return existingList;
            });
        return Task.CompletedTask;
    }

    public Task CreateSession(Guid userId, string refreshToken, string ipAddress, string deviceInfo)
    {
        var session = new ActiveSession
        {
            UserId = userId.ToString(),
            RefreshToken = refreshToken,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo,
            CreatedAt = DateTime.UtcNow
        };
        
        return AddSession(session);
    }

    public Task RemoveSession(string userId, string refreshToken)
    {
        if (_activeSessions.TryGetValue(userId, out var sessions))
        {
            sessions.RemoveAll(s => s.RefreshToken == refreshToken);
            if (!sessions.Any()) _activeSessions.TryRemove(userId, out _);
        }
        return Task.CompletedTask;
    }

    public Task<bool> IsActiveSession(string userId, string refreshToken)
    {
        if (_activeSessions.TryGetValue(userId, out var sessions))
            return Task.FromResult(sessions.Any(s => s.RefreshToken == refreshToken));
        return Task.FromResult(false);
    }

    public Task<IEnumerable<ActiveSession>> GetUserSessions(string userId)
    {
        return Task.FromResult(
            _activeSessions.TryGetValue(userId, out var sessions)
                ? sessions.AsEnumerable()
                : Enumerable.Empty<ActiveSession>());
    }

    public Task RevokeAllSessions(string userId)
    {
        _activeSessions.TryRemove(userId, out _);
        return Task.CompletedTask;
    }

    public Task<ActiveSession> GetSessionByRefreshToken(string refreshToken)
    {
        var session = _activeSessions.Values
            .SelectMany(s => s)
            .FirstOrDefault(s => s.RefreshToken == refreshToken);
        return Task.FromResult(session);
    }
}