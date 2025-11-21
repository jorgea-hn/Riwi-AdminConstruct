using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Web.ViewModels
{
    public class SaleViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        
        public Guid CustomerId { get; set; }

        public DateTime Date { get; set; }
        public string CustomerName { get; set; } = string.Empty;


        public decimal TotalAmount { get; set; }

        public string? ReceiptUrl { get; set; }

        public List<SaleDetailViewModel> Details { get; set; } = new();
    }
}
