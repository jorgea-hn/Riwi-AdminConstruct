using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Web.ViewModels
{
    public class MachineryRentalViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar una maquinaria")]
        [Display(Name = "Maquinaria")]
        public int MachineryId { get; set; }
        
        public string? MachineryName { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        [Display(Name = "Cliente")]
        public Guid CustomerId { get; set; }
        
        public string? CustomerName { get; set; }
        
        [Required(ErrorMessage = "La fecha y hora de inicio son obligatorias")]
        [Display(Name = "Fecha y Hora de Inicio")]
        public DateTime StartDateTime { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "La fecha y hora de fin son obligatorias")]
        [Display(Name = "Fecha y Hora de Fin")]
        public DateTime EndDateTime { get; set; } = DateTime.Now.AddDays(1);
        
        [Required(ErrorMessage = "El precio por día es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        [Display(Name = "Precio por Día")]
        public decimal PricePerDay { get; set; }
        
        [Display(Name = "Monto Total")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; }
        
        [Display(Name = "Fecha de Devolución Real")]
        public DateTime? ActualReturnDate { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Notas")]
        public string? Notes { get; set; }
        
        [Display(Name = "Días de Alquiler")]
        public int RentalDays { get; set; }
    }
}
