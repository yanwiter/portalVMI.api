using System.ComponentModel.DataAnnotations;

namespace Vmi.Portal.Entities;

public class ColumnPreference
{
    [Key]
    public Guid Id { get; set; }
    
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
    public string Columns { get; set; }
    
    public bool IsFavorite { get; set; }
    
    public bool IsPinned { get; set; }
    
    public bool IsDefault { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    [Required]
    [StringLength(50)]
    public string CreatedBy { get; set; }
}
