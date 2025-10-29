namespace AdminConstruct.Ryzor.Models;

public class SaleDetail
{

    public int Id { get; set; }
    
    public Guid SaleId { get; set; }
    
    public Sale Sale { get; set; } = null!;
    
    public Guid ProductId { get; set; }
    
    public Product Product { get; set; } = null!;
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
}