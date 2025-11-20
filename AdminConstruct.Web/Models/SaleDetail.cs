using System.ComponentModel.DataAnnotations.Schema;

namespace AdminConstruct.Web.Models;

public class SaleDetail
{

    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid SaleId { get; set; }
    
    public Sale Sale { get; set; } = null!;
    
    public Guid ProductId { get; set; }
    
    public Product Product { get; set; } = null!;
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal Subtotal => Quantity * UnitPrice;
}