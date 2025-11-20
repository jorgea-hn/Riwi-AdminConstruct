namespace AdminConstruct.Web.Models;

public class Machinery
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty; // e.g., "Excavadora CAT 320D"
    public string Category { get; set; } = string.Empty; // e.g., "Excavadora", "Cargador Frontal"
    public string Brand { get; set; } = string.Empty; // e.g., "Caterpillar", "Komatsu"
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string SerialNumber { get; set; } = string.Empty; // Unique identifier for the machine
    public decimal RentalPricePerHour { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
}
