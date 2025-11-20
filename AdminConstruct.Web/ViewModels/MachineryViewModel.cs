using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Web.ViewModels
{
    public class MachineryViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre de la maquinaria es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede tener más de 150 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(100, ErrorMessage = "La categoría no puede tener más de 100 caracteres.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(100, ErrorMessage = "La marca no puede tener más de 100 caracteres.")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El modelo no puede tener más de 100 caracteres.")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(1950, 2100, ErrorMessage = "El año debe ser válido.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "El número de serie es obligatorio.")]
        [StringLength(100, ErrorMessage = "El número de serie no puede tener más de 100 caracteres.")]
        public string SerialNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio de alquiler por hora es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio de alquiler por hora debe ser mayor que cero.")]
        [Display(Name = "Precio de Alquiler por Hora")]
        public decimal RentalPricePerHour { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Display(Name = "URL de la Imagen")]
        public string? ImageUrl { get; set; }

        [StringLength(1000, ErrorMessage = "La descripción no puede tener más de 1000 caracteres.")]
        public string? Description { get; set; }
    }
}
