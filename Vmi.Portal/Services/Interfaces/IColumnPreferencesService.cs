using Vmi.Portal.Entities;
using Vmi.Portal.Models;

namespace Vmi.Portal.Services.Interfaces;

public interface IColumnPreferencesService
{
    Task<Result<IEnumerable<ColumnPreference>>> GetAllAsync();
    Task<Result<ColumnPreference>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<ColumnPreference>>> GetByUserAsync(string idUser);
    Task<Result<IEnumerable<ColumnPreference>>> GetByScreenKeyAsync(string screenKey);
    Task<Result<IEnumerable<ColumnPreference>>> GetByUserAndScreenAsync(string idUser, string screenKey);
    Task<Result<ColumnPreference>> GetDefaultAsync(string screenKey, string idUser);
    Task<Result<ColumnPreference>> CreateAsync(ColumnPreferenceCreateRequest request);
    Task<Result<ColumnPreference>> UpdateAsync(Guid id, ColumnPreferenceUpdateRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<bool>> SetAsDefaultAsync(Guid id, string screenKey, string idUser);
    Task<Result<bool>> ToggleFavoriteAsync(Guid id);
    Task<Result<bool>> TogglePinnedAsync(Guid id);
}
