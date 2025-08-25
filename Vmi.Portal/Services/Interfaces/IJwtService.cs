using System.Security.Claims;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Usuario usuario);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task<TokenResponse> GenerateTokens(Usuario usuario, string ipAddress, string deviceInfo);
}