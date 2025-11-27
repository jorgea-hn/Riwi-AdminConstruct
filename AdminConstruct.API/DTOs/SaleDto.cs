namespace AdminConstruct.API.DTOs;

public class SaleDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }

    public List<SaleDetailDto> Details { get; set; } = new();
}