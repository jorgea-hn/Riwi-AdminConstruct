using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Ryzor.ViewModels;

public class SaleViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un cliente.")]
    public Guid CustomerId { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required(ErrorMessage = "Debe agregar al menos un producto a la venta.")]
    public List<SaleDetailViewModel> Details { get; set; } = new();
}