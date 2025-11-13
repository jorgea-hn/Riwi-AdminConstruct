using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Ryzor.ViewModels;

public class ProductViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "La cantidad en stock es obligatoria.")]
    [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
    public int StockQuantity { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Description { get; set; }
}