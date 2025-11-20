using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Web.ViewModels
{
    public class SaleDetailViewModel
    {
        public Guid Id { get; set; }

        public Guid SaleId { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty; // For display purposes

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que cero.")]
        public decimal UnitPrice { get; set; }

        public decimal Subtotal => Quantity * UnitPrice;
    }
}
