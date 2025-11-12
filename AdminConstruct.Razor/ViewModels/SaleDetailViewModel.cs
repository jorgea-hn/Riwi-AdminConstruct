using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Ryzor.ViewModels;

public class SaleDetailViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un producto.")]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "El precio unitario es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que 0.")]
    public decimal UnitPrice { get; set; }
    
    public string ProductName { get; set; } = string.Empty; 
}