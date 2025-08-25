using Dapper;
using Vmi.Portal.DbContext;
using Vmi.Portal.Entities;
using Vmi.Portal.Repositories.Interfaces;

namespace Vmi.Portal.Repositories;

public class ColumnPreferencesRepository : IColumnPreferencesRepository
{
    private readonly VmiDbContext _vmiDbContext;

    public ColumnPreferencesRepository(VmiDbContext vmiDbContext)
    {
        _vmiDbContext = vmiDbContext;
    }

    public async Task<IEnumerable<ColumnPreference>> GetAllAsync()
    {
        const string sql = @"
            SELECT 
                Id,
                NamePreference,
                Description,
                ScreenKey,
                IdUser,
                Columns,
                IsFavorite,
                IsPinned,
                IsDefault,
                CreatedAt,
                UpdatedAt,
                CreatedBy
            FROM ColumnPreferences
            ORDER BY CreatedAt DESC";

        return await _vmiDbContext.Connection.QueryAsync<ColumnPreference>(sql);
    }

    public async Task<ColumnPreference> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT 
                Id,
                NamePreference,
                Description,
                ScreenKey,
                IdUser,
                Columns,
                IsFavorite,
                IsPinned,
                IsDefault,
                CreatedAt,
                UpdatedAt,
                CreatedBy
            FROM ColumnPreferences
            WHERE Id = @Id";

        return await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<ColumnPreference>(sql, new { Id = id });
    }

    public async Task<IEnumerable<ColumnPreference>> GetByUserAsync(string idUser)
    {
        const string sql = @"
            SELECT 
                Id,
                NamePreference,
                Description,
                ScreenKey,
                IdUser,
                Columns,
                IsFavorite,
                IsPinned,
                IsDefault,
                CreatedAt,
                UpdatedAt,
                CreatedBy
            FROM ColumnPreferences
            WHERE IdUser = @IdUser
            ORDER BY IsDefault DESC, IsPinned DESC, IsFavorite DESC, CreatedAt DESC";

        return await _vmiDbContext.Connection.QueryAsync<ColumnPreference>(sql, new { IdUser = idUser });
    }

    public async Task<IEnumerable<ColumnPreference>> GetByScreenKeyAsync(string screenKey)
    {
        const string sql = @"
            SELECT 
                Id,
                NamePreference,
                Description,
                ScreenKey,
                IdUser,
                Columns,
                IsFavorite,
                IsPinned,
                IsDefault,
                CreatedAt,
                UpdatedAt,
                CreatedBy
            FROM ColumnPreferences
            WHERE ScreenKey = @ScreenKey
            ORDER BY IsDefault DESC, IsPinned DESC, IsFavorite DESC, CreatedAt DESC";

        return await _vmiDbContext.Connection.QueryAsync<ColumnPreference>(sql, new { ScreenKey = screenKey });
    }

    public async Task<IEnumerable<ColumnPreference>> GetByUserAndScreenAsync(string idUser, string screenKey)
    {
        const string sql = @"
            SELECT 
                Id,
                NamePreference,
                Description,
                ScreenKey,
                IdUser,
                Columns,
                IsFavorite,
                IsPinned,
                IsDefault,
                CreatedAt,
                UpdatedAt,
                CreatedBy
            FROM ColumnPreferences
            WHERE IdUser = @IdUser AND ScreenKey = @ScreenKey
            ORDER BY IsDefault DESC, IsPinned DESC, IsFavorite DESC, CreatedAt DESC";

        return await _vmiDbContext.Connection.QueryAsync<ColumnPreference>(sql, new { IdUser = idUser, ScreenKey = screenKey });
    }

    public async Task<ColumnPreference> GetDefaultAsync(string screenKey, string idUser)
    {
        const string sql = @"
            SELECT 
                Id,
                NamePreference,
                Description,
                ScreenKey,
                IdUser,
                Columns,
                IsFavorite,
                IsPinned,
                IsDefault,
                CreatedAt,
                UpdatedAt,
                CreatedBy
            FROM ColumnPreferences
            WHERE ScreenKey = @ScreenKey AND IdUser = @IdUser AND IsDefault = 1";

        return await _vmiDbContext.Connection.QueryFirstOrDefaultAsync<ColumnPreference>(sql, new { ScreenKey = screenKey, IdUser = idUser });
    }

    public async Task<ColumnPreference> CreateAsync(ColumnPreference columnPreference)
    {
        const string sql = @"
            INSERT INTO ColumnPreferences (
                Id, NamePreference, Description, ScreenKey, IdUser, 
                Columns, IsFavorite, IsPinned, IsDefault, 
                CreatedAt, UpdatedAt, CreatedBy
            ) VALUES (
                @Id, @NamePreference, @Description, @ScreenKey, @IdUser,
                @Columns, @IsFavorite, @IsPinned, @IsDefault,
                @CreatedAt, @UpdatedAt, @CreatedBy
            );
            
            SELECT 
                Id, NamePreference, Description, ScreenKey, IdUser,
                Columns, IsFavorite, IsPinned, IsDefault,
                CreatedAt, UpdatedAt, CreatedBy
            FROM ColumnPreferences
            WHERE Id = @Id";

        return await _vmiDbContext.Connection.QueryFirstAsync<ColumnPreference>(sql, columnPreference);
    }

    public async Task<ColumnPreference> UpdateAsync(Guid id, ColumnPreference columnPreference)
    {
        const string sql = @"
            UPDATE ColumnPreferences SET
                NamePreference = @NamePreference,
                Description = @Description,
                Columns = @Columns,
                IsFavorite = @IsFavorite,
                IsPinned = @IsPinned,
                IsDefault = @IsDefault,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id;
            
            SELECT 
                Id, NamePreference, Description, ScreenKey, IdUser,
                Columns, IsFavorite, IsPinned, IsDefault,
                CreatedAt, UpdatedAt, CreatedBy
            FROM ColumnPreferences
            WHERE Id = @Id";

        columnPreference.Id = id;
        return await _vmiDbContext.Connection.QueryFirstAsync<ColumnPreference>(sql, columnPreference);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM ColumnPreferences WHERE Id = @Id";
        
        return await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<bool> SetAsDefaultAsync(Guid id, string screenKey, string idUser)
    {
        const string sql = @"
            UPDATE ColumnPreferences SET IsDefault = 0 WHERE ScreenKey = @ScreenKey AND IdUser = @IdUser;
            UPDATE ColumnPreferences SET IsDefault = 1, UpdatedAt = @UpdatedAt WHERE Id = @Id";
        
        var rowsAffected = await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id, ScreenKey = screenKey, IdUser = idUser, UpdatedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }

    public async Task<bool> ToggleFavoriteAsync(Guid id)
    {
        const string sql = @"
            UPDATE ColumnPreferences 
            SET IsFavorite = ~IsFavorite, UpdatedAt = @UpdatedAt 
            WHERE Id = @Id";
        
        var rowsAffected = await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }

    public async Task<bool> TogglePinnedAsync(Guid id)
    {
        const string sql = @"
            UPDATE ColumnPreferences 
            SET IsPinned = ~IsPinned, UpdatedAt = @UpdatedAt 
            WHERE Id = @Id";
        
        var rowsAffected = await _vmiDbContext.Connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }
}
