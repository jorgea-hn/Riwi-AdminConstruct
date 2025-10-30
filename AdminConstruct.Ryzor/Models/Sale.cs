namespace AdminConstruct.Ryzor.Models;

public class Sale
{

    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CustomerId { get; set; }
    
    public Customer Customer { get; set; } = null!;


    public DateTime Date { get; set; } = DateTime.UtcNow;

    public List<SaleDetail> Details { get; set; } = new();
}