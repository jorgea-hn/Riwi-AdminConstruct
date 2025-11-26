
using AdminConstruct.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Machinery> Machineries { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleDetail> SaleDetails { get; set; }
    public DbSet<MachineryRental> MachineryRentals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurar relación Customer-User
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.UserId);
        
        // Configurar relaciones de MachineryRental
        modelBuilder.Entity<MachineryRental>()
            .HasOne(r => r.Machinery)
            .WithMany()
            .HasForeignKey(r => r.MachineryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<MachineryRental>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
