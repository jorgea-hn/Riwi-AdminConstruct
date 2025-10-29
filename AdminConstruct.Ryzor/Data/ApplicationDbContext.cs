using AdminConstruct.Ryzor.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.Ryzor.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleDetail> SaleDetails { get; set; }
}