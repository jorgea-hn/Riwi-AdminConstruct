using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdminConstruct.Web.Models;

namespace AdminConstruct.Web.Models
{
    public class MachineryRental
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required(ErrorMessage = "La maquinaria es obligatoria.")]
        public int MachineryId { get; set; }
        
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public Guid CustomerId { get; set; }
        
        [Required(ErrorMessage = "La fecha y hora de inicio son obligatorias.")]
        [Display(Name = "Fecha y Hora de Inicio")]
        public DateTime StartDateTime { get; set; }
        
        [Required(ErrorMessage = "La fecha y hora de fin son obligatorias.")]
        [Display(Name = "Fecha y Hora de Fin")]
        public DateTime EndDateTime { get; set; }
        
        [Required(ErrorMessage = "El precio por día es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        [Display(Name = "Precio por Día")]
        public decimal PricePerDay { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Monto Total")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Fecha de Devolución Real")]
        public DateTime? ActualReturnDate { get; set; }
        
        [StringLength(500)]
        [Display(Name = "Notas")]
        public string? Notes { get; set; }
        
        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navegación
        public Machinery Machinery { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}
