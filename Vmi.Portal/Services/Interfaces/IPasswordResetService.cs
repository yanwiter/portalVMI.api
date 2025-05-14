namespace Vmi.Portal.Services.Interfaces;

public interface IPasswordResetService
{
    Task<(bool Success, string Message)> GerarTokenRedefinicaoSenha(string email);
    Task<bool> ValidarTokenRedefinicaoSenha(string token);
    Task<(bool Success, string Message)> RedefinirSenha(string token, string newPassword);
}
