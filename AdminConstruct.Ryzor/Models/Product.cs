using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Ryzor.Models;

public class Product
{

    public Guid Id { get; set; } = Guid.NewGuid(); 
    
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;
    
    [Range(0, 1_000_000)]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}