namespace AdminConstruct.Ryzor.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();


    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;


    public string? Phone { get; set; }
    
    public string Document { get; set; } = string.Empty;
}