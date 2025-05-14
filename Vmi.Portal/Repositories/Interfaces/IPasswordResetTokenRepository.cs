using System.Threading.Tasks;
using Vmi.Portal.Entities;

namespace Vmi.Portal.Data.Repositories.Interfaces;
public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken> PegarTokenValidoAsync(string token);
    Task CriarTokenRedefinicaoSenhaAsync(PasswordResetToken token);
    Task AtualizarTokenRedefinicaoSenhaAsync(PasswordResetToken token);
}
