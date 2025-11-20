namespace AdminConstruct.Web.Models;

public class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CustomerId { get; set; }
    
    public Customer Customer { get; set; } = null!;

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public string? ReceiptUrl { get; set; }

    public List<SaleDetail> Details { get; set; } = new();
}