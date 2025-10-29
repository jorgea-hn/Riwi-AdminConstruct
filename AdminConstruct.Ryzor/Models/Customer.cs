using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.Ryzor.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();


    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;


    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }
    
    [Required]
    [StringLength(30)]
    public string Document { get; set; } = string.Empty;
}