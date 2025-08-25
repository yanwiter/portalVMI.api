using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Vmi.Portal.Entities;
using Vmi.Portal.Services.Interfaces;

namespace Vmi.Portal.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly IActiveSessionService _activeSessionService;

    public JwtService(IConfiguration configuration, IActiveSessionService activeSessionService)
    {
        _configuration = configuration;
        _activeSessionService = activeSessionService;
    }

    public string GenerateAccessToken(Usuario usuario)
    {
        if (usuario == null)
            throw new ArgumentNullException(nameof(usuario));

        if (string.IsNullOrWhiteSpace(_configuration["JwtSettings:Secret"]))
            throw new ArgumentException("JWT Secret não configurado");

        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim("Perfil", usuario.PerfilNome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>("JwtSettings:ExpiryMinutes")),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public async Task<TokenResponse> GenerateTokens(Usuario usuario, string ipAddress, string deviceInfo)
    {
        var accessToken = GenerateAccessToken(usuario);
        var refreshToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>("JwtSettings:ExpiryMinutes"));

        await _activeSessionService.AddSession(new ActiveSession
        {
            UserId = usuario.Id.ToString(),
            Email = usuario.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo
        });

        return new TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken, ExpiresAt = expiresAt };
    }
}