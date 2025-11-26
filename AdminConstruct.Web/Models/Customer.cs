using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AdminConstruct.Web.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    
    // Relación con IdentityUser
    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }
}