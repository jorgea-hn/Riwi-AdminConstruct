namespace AdminConstruct.Web.Models;

public class Product
{

    public Guid Id { get; set; } = Guid.NewGuid(); 
    
    public string Name { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public int StockQuantity { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }
}