using Microsoft.AspNetCore.Mvc;
using Vmi.Portal.Entities;
using Vmi.Portal.Models;
using Vmi.Portal.Services.Interfaces;
using System.Security.Claims;

namespace Vmi.Portal.Controllers;

[ApiController]
[Route("[controller]")]
public class ColumnPreferencesController : ControllerBase
{
    private readonly IColumnPreferencesService _columnPreferencesService;
    private readonly ILogger<ColumnPreferencesController> _logger;

    public ColumnPreferencesController(
        IColumnPreferencesService columnPreferencesService,
        ILogger<ColumnPreferencesController> logger)
    {
        _columnPreferencesService = columnPreferencesService;
        _logger = logger;
    }

    private string GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            userIdClaim = User.FindFirst("userId")?.Value;
        }
        return userIdClaim;
    }

    [HttpGet]
    public async Task<ActionResult<Result<IEnumerable<ColumnPreference>>>> GetAll()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<IEnumerable<ColumnPreference>>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            var result = await _columnPreferencesService.GetByUserAsync(currentUserId);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferências de colunas do usuário atual");
            return StatusCode(500, new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpGet("user-screen/{screenKey}")]
    public async Task<ActionResult<Result<IEnumerable<ColumnPreference>>>> GetByUserAndScreen(string screenKey)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<IEnumerable<ColumnPreference>>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            var result = await _columnPreferencesService.GetByUserAndScreenAsync(currentUserId, screenKey);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferências de colunas do usuário {UserId} para tela {ScreenKey}", GetCurrentUserId(), screenKey);
            return StatusCode(500, new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<ColumnPreference>>> GetById(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<ColumnPreference>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            var result = await _columnPreferencesService.GetByIdAsync(id);
            
            if (result.IsSuccess && result.Data != null)
            {
                if (result.Data.IdUser != currentUserId)
                {
                    return Forbid();
                }

                return Ok(result);
            }
            
            if (result.Error?.Code == "NOT_FOUND")
            {
                return NotFound(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferência de coluna com ID {Id}", id);
            return StatusCode(500, new Result<ColumnPreference>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpGet("user/{idUser}")]
    public async Task<ActionResult<Result<IEnumerable<ColumnPreference>>>> GetByUser(string idUser)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<IEnumerable<ColumnPreference>>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            if (idUser != currentUserId)
            {
                return Forbid();
            }

            var result = await _columnPreferencesService.GetByUserAsync(idUser);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferências de colunas do usuário {IdUser}", idUser);
            return StatusCode(500, new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpGet("screen/{screenKey}")]
    public async Task<ActionResult<Result<IEnumerable<ColumnPreference>>>> GetByScreenKey(string screenKey)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<IEnumerable<ColumnPreference>>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            var result = await _columnPreferencesService.GetByUserAndScreenAsync(currentUserId, screenKey);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferências de colunas da tela {ScreenKey} para usuário {UserId}", screenKey, GetCurrentUserId());
            return StatusCode(500, new Result<IEnumerable<ColumnPreference>>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpGet("screen/{screenKey}/default")]
    public async Task<ActionResult<Result<ColumnPreference>>> GetDefault(string screenKey)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<ColumnPreference>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            var result = await _columnPreferencesService.GetDefaultAsync(screenKey, currentUserId);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            if (result.Error?.Code == "NOT_FOUND")
            {
                return NotFound(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter preferência padrão da tela {ScreenKey} para usuário {UserId}", screenKey, GetCurrentUserId());
            return StatusCode(500, new Result<ColumnPreference>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<Result<ColumnPreference>>> Create([FromBody] ColumnPreferenceCreateRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<ColumnPreference>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new Result<ColumnPreference>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Dados de entrada inválidos",
                        Code = "VALIDATION_ERROR"
                    }
                });
            }

            request.IdUser = currentUserId;
            request.CreatedBy = currentUserId;

            var result = await _columnPreferencesService.CreateAsync(request);
            
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar preferência de coluna");
            return StatusCode(500, new Result<ColumnPreference>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Result<ColumnPreference>>> Update(Guid id, [FromBody] ColumnPreferenceUpdateRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<ColumnPreference>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new Result<ColumnPreference>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Dados de entrada inválidos",
                        Code = "VALIDATION_ERROR"
                    }
                });
            }

            var existingPreference = await _columnPreferencesService.GetByIdAsync(id);
            if (existingPreference.IsSuccess && existingPreference.Data != null)
            {
                if (existingPreference.Data.IdUser != currentUserId)
                {
                    return Forbid();
                }
            }

            var result = await _columnPreferencesService.UpdateAsync(id, request);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            if (result.Error?.Code == "NOT_FOUND")
            {
                return NotFound(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar preferência de coluna com ID {Id}", id);
            return StatusCode(500, new Result<ColumnPreference>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            var existingPreference = await _columnPreferencesService.GetByIdAsync(id);
            if (existingPreference.IsSuccess && existingPreference.Data != null)
            {
                if (existingPreference.Data.IdUser != currentUserId)
                {
                    return Forbid();
                }
            }

            var result = await _columnPreferencesService.DeleteAsync(id);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            if (result.Error?.Code == "NOT_FOUND")
            {
                return NotFound(result);
            }
            
            if (result.Error?.Code == "VALIDATION_ERROR")
            {
                return BadRequest(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover preferência de coluna com ID {Id}", id);
            return StatusCode(500, new Result<bool>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpPatch("{id:guid}/set-default")]
    public async Task<ActionResult<Result<bool>>> SetAsDefault(Guid id, [FromQuery] string screenKey)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            if (string.IsNullOrEmpty(screenKey))
            {
                return BadRequest(new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "ScreenKey é obrigatório",
                        Code = "VALIDATION_ERROR"
                    }
                });
            }

            var existingPreference = await _columnPreferencesService.GetByIdAsync(id);
            if (existingPreference.IsSuccess && existingPreference.Data != null)
            {
                if (existingPreference.Data.IdUser != currentUserId)
                {
                    return Forbid();
                }
            }

            var result = await _columnPreferencesService.SetAsDefaultAsync(id, screenKey, currentUserId);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao definir preferência padrão com ID {Id} para tela {ScreenKey}", id, screenKey);
            return StatusCode(500, new Result<bool>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpPatch("{id:guid}/toggle-favorite")]
    public async Task<ActionResult<Result<bool>>> ToggleFavorite(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            var existingPreference = await _columnPreferencesService.GetByIdAsync(id);
            if (existingPreference.IsSuccess && existingPreference.Data != null)
            {
                if (existingPreference.Data.IdUser != currentUserId)
                {
                    return Forbid();
                }
            }

            var result = await _columnPreferencesService.ToggleFavoriteAsync(id);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alternar favorito da preferência com ID {Id}", id);
            return StatusCode(500, new Result<bool>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }

    [HttpPatch("{id:guid}/toggle-pinned")]
    public async Task<ActionResult<Result<bool>>> TogglePinned(Guid id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(new Result<bool>
                {
                    IsSuccess = false,
                    Error = new ErrorDetails
                    {
                        Message = "Usuário não autenticado",
                        Code = "UNAUTHORIZED"
                    }
                });
            }

            var existingPreference = await _columnPreferencesService.GetByIdAsync(id);
            if (existingPreference.IsSuccess && existingPreference.Data != null)
            {
                if (existingPreference.Data.IdUser != currentUserId)
                {
                    return Forbid();
                }
            }

            var result = await _columnPreferencesService.TogglePinnedAsync(id);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alternar fixação da preferência com ID {Id}", id);
            return StatusCode(500, new Result<bool>
            {
                IsSuccess = false,
                Error = new ErrorDetails
                {
                    Message = "Erro interno do servidor",
                    Code = "INTERNAL_SERVER_ERROR"
                }
            });
        }
    }
}
