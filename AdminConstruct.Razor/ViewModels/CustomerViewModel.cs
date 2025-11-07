using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Ryzor.ViewModels;

public class CustomerViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento es obligatorio.")]
    [StringLength(20, ErrorMessage = "El documento no puede tener más de 20 caracteres.")]
    public string Document { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Número de teléfono inválido.")]
    public string? Phone { get; set; }
}