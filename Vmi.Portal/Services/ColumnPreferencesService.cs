using Vmi.Portal.Entities;
using Vmi.Portal.Models;
using Vmi.Portal.Repositories.Interfaces;
using Vmi.Portal.Services.Interfaces;

namespace Vmi.Portal.Services;

public class ColumnPreferencesService : IColumnPreferencesService
{
    private readonly IColumnPreferencesRepository _columnPreferencesRepository;
    private readonly ILogger<ColumnPreferencesService> _logger;

    public ColumnPreferencesService(
        IColumnPreferencesRepository columnPreferencesRepository,
        ILogger<ColumnPreferencesService> logger)
    {
        _columnPreferencesRepository = columnPreferencesRepository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ColumnPreference>>> GetAllAsync()
    {
        try
        {
            var preferences = await _columnPreferencesRepository.GetAllAsync();
            return new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = true,
                Data = preferences
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as preferências de colunas");
            return new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao obter preferências de colunas",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<ColumnPreference>> GetByIdAsync(Guid id)
    {
        try
        {
            var preference = await _columnPreferencesRepository.GetByIdAsync(id);
            
            if (preference == null)
            {
                return new Result<ColumnPreference>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Preferência de coluna não encontrada",
                        Code = "NOT_FOUND"
                    }
                };
            }

            return new Result<ColumnPreference>
            {
                IsSuccess = true,
                Data = preference
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferência de coluna com ID {Id}", id);
            return new Result<ColumnPreference>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao obter preferência de coluna",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<IEnumerable<ColumnPreference>>> GetByUserAsync(string idUser)
    {
        try
        {
            var preferences = await _columnPreferencesRepository.GetByUserAsync(idUser);
            return new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = true,
                Data = preferences
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferências de colunas do usuário {IdUser}", idUser);
            return new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao obter preferências do usuário",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<IEnumerable<ColumnPreference>>> GetByScreenKeyAsync(string screenKey)
    {
        try
        {
            var preferences = await _columnPreferencesRepository.GetByScreenKeyAsync(screenKey);
            return new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = true,
                Data = preferences
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferências de colunas da tela {ScreenKey}", screenKey);
            return new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao obter preferências da tela",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<IEnumerable<ColumnPreference>>> GetByUserAndScreenAsync(string idUser, string screenKey)
    {
        try
        {
            var preferences = await _columnPreferencesRepository.GetByUserAndScreenAsync(idUser, screenKey);
            return new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = true,
                Data = preferences
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferências de colunas do usuário {IdUser} para tela {ScreenKey}", idUser, screenKey);
            return new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao obter preferências do usuário para a tela",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<ColumnPreference>> GetDefaultAsync(string screenKey, string idUser)
    {
        try
        {
            var preference = await _columnPreferencesRepository.GetDefaultAsync(screenKey, idUser);
            
            if (preference == null)
            {
                return new Result<ColumnPreference>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Preferência padrão não encontrada para esta tela e usuário",
                        Code = "NOT_FOUND"
                    }
                };
            }

            return new Result<ColumnPreference>
            {
                IsSuccess = true,
                Data = preference
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferência padrão da tela {ScreenKey} para usuário {IdUser}", screenKey, idUser);
            return new Result<ColumnPreference>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao obter preferência padrão",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<ColumnPreference>> CreateAsync(ColumnPreferenceCreateRequest request)
    {
        try
        {
            var columnPreference = new ColumnPreference
            {
                Id = Guid.NewGuid(),
                NamePreference = request.NamePreference,
                Description = request.Description,
                ScreenKey = request.ScreenKey,
                IdUser = request.IdUser,
                Columns = request.Columns,
                IsFavorite = request.IsFavorite,
                IsPinned = request.IsPinned,
                IsDefault = request.IsDefault,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy
            };

            var created = await _columnPreferencesRepository.CreateAsync(columnPreference);
            
            return new Result<ColumnPreference>
            {
                IsSuccess = true,
                Data = created
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar preferência de coluna");
            return new Result<ColumnPreference>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao criar preferência de coluna",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<ColumnPreference>> UpdateAsync(Guid id, ColumnPreferenceUpdateRequest request)
    {
        try
        {
            var existingPreference = await _columnPreferencesRepository.GetByIdAsync(id);
            
            if (existingPreference == null)
            {
                return new Result<ColumnPreference>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Preferência de coluna não encontrada",
                        Code = "NOT_FOUND"
                    }
                };
            }

            var columnPreference = new ColumnPreference
            {
                Id = id,
                NamePreference = request.NamePreference,
                Description = request.Description,
                ScreenKey = existingPreference.ScreenKey,
                IdUser = existingPreference.IdUser,
                Columns = request.Columns,
                IsFavorite = request.IsFavorite,
                IsPinned = request.IsPinned,
                IsDefault = request.IsDefault,
                CreatedAt = existingPreference.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = existingPreference.CreatedBy
            };
            
            var updatedPreference = await _columnPreferencesRepository.UpdateAsync(id, columnPreference);
            
            return new Result<ColumnPreference>
            {
                IsSuccess = true,
                Data = updatedPreference
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar preferência de coluna com ID {Id}", id);
            return new Result<ColumnPreference>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao atualizar preferência de coluna",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var existing = await _columnPreferencesRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Preferência de coluna não encontrada",
                        Code = "NOT_FOUND"
                    }
                };
            }

            if (existing.IsDefault)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Não é possível remover a preferência padrão",
                        Code = "VALIDATION_ERROR"
                    }
                };
            }

            var success = await _columnPreferencesRepository.DeleteAsync(id);
            
            return new Result<bool>
            {
                IsSuccess = true,
                Data = success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover preferência de coluna com ID {Id}", id);
            return new Result<bool>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao remover preferência de coluna",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<bool>> SetAsDefaultAsync(Guid id, string screenKey, string idUser)
    {
        try
        {
            var success = await _columnPreferencesRepository.SetAsDefaultAsync(id, screenKey, idUser);
            
            return new Result<bool>
            {
                IsSuccess = true,
                Data = success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao definir preferência padrão com ID {Id} para tela {ScreenKey} e usuário {IdUser}", id, screenKey, idUser);
            return new Result<bool>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao definir preferência padrão",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<bool>> ToggleFavoriteAsync(Guid id)
    {
        try
        {
            var success = await _columnPreferencesRepository.ToggleFavoriteAsync(id);
            
            return new Result<bool>
            {
                IsSuccess = true,
                Data = success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alternar favorito da preferência com ID {Id}", id);
            return new Result<bool>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao alternar favorito",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }

    public async Task<Result<bool>> TogglePinnedAsync(Guid id)
    {
        try
        {
            var success = await _columnPreferencesRepository.TogglePinnedAsync(id);
            
            return new Result<bool>
            {
                IsSuccess = true,
                Data = success
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alternar fixação da preferência com ID {Id}", id);
            return new Result<bool>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno ao alternar fixação",
                    Code = "INTERNAL_ERROR"
                }
            };
        }
    }
}
