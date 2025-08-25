using System.ComponentModel.DataAnnotations;

namespace Vmi.Portal.Models;

public class ColumnPreferenceCreateRequest
{
    [Required]
    [StringLength(100)]
    public string NamePreference { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [StringLength(50)]
    public string ScreenKey { get; set; }
    
    [Required]
    [StringLength(50)]
    public string IdUser { get; set; }
    
    [Required]
    public string Columns { get; set; } // JSON serializado das colunas
    
    public bool IsFavorite { get; set; }
    
    public bool IsPinned { get; set; }
    
    public bool IsDefault { get; set; }
    
    [Required]
    [StringLength(50)]
    public string CreatedBy { get; set; }
}
