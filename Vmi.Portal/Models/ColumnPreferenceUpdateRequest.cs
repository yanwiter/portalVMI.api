using System.ComponentModel.DataAnnotations;

namespace Vmi.Portal.Models;

public class ColumnPreferenceUpdateRequest
{
    [Required]
    [StringLength(100)]
    public string NamePreference { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public string Columns { get; set; } // JSON serializado das colunas
    
    public bool IsFavorite { get; set; }
    
    public bool IsPinned { get; set; }
    
    public bool IsDefault { get; set; }
}
