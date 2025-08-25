using Vmi.Portal.Entities;

namespace Vmi.Portal.Repositories.Interfaces;

public interface IColumnPreferencesRepository
{
    Task<IEnumerable<ColumnPreference>> GetAllAsync();
    Task<ColumnPreference> GetByIdAsync(Guid id);
    Task<IEnumerable<ColumnPreference>> GetByUserAsync(string idUser);
    Task<IEnumerable<ColumnPreference>> GetByScreenKeyAsync(string screenKey);
    Task<IEnumerable<ColumnPreference>> GetByUserAndScreenAsync(string idUser, string screenKey);
    Task<ColumnPreference> GetDefaultAsync(string screenKey, string idUser);
    Task<ColumnPreference> CreateAsync(ColumnPreference columnPreference);
    Task<ColumnPreference> UpdateAsync(Guid id, ColumnPreference columnPreference);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> SetAsDefaultAsync(Guid id, string screenKey, string idUser);
    Task<bool> ToggleFavoriteAsync(Guid id);
    Task<bool> TogglePinnedAsync(Guid id);
}
