using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.API.DTOs
{
    public class MachineryRentalDto
    {
        public Guid Id { get; set; }
        public int MachineryId { get; set; }
        public string MachineryName { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateMachineryRentalDto
    {
        [Required]
        public int MachineryId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        public string? Notes { get; set; }
    }
}
