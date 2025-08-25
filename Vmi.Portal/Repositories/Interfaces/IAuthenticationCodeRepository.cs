namespace Vmi.Portal.Repositories.Interfaces;

public interface IAuthenticationCodeRepository
{
    Task SaveAuthCodeAsync(Guid userId, string code, DateTime expiration);
    Task<bool> ValidateCodeAsync(Guid userId, string code);
    Task DeleteUserCodesAsync(Guid userId);
    Task<int> GetUserCodeCountAsync(Guid userId);
    Task CleanExpiredCodesAsync();
}
