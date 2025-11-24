using Microsoft.AspNetCore.Identity;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;

namespace AdminConstruct.Web;

public class SeedData
{
    public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        // Roles por defecto
        string[] roles = { "Administrador", "Cliente" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Usuario administrador
        string adminEmail = "admin@firmeza.com";
        string adminPass = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, adminPass);
            await userManager.AddToRoleAsync(admin, "Administrador");
        }

        // Seed Data for Products
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Cemento Argos", Price = 25000, StockQuantity = 100, Description = "Cemento gris de uso general", ImageUrl = "/images/products/cemento.jpg" },
                new Product { Name = "Ladrillo Rojo", Price = 1200, StockQuantity = 5000, Description = "Ladrillo cocido estándar", ImageUrl = "/images/products/ladrillo.jpg" },
                new Product { Name = "Varilla 1/2", Price = 18000, StockQuantity = 200, Description = "Varilla corrugada de acero", ImageUrl = "/images/products/varilla.jpg" },
                new Product { Name = "Arena de Río", Price = 80000, StockQuantity = 50, Description = "Arena lavada por metro cúbico", ImageUrl = "/images/products/arena.jpg" },
                new Product { Name = "Grava", Price = 90000, StockQuantity = 40, Description = "Grava triturada por metro cúbico", ImageUrl = "/images/products/grava.jpg" }
            );
            await context.SaveChangesAsync();
        }

        // Seed Data for Machinery
        if (!context.Machineries.Any())
        {
            context.Machineries.AddRange(
                new Machinery { Name = "Mezcladora de Concreto", Description = "Mezcladora de 1 bulto a gasolina", Stock = 5, Price = 50000, IsActive = true },
                new Machinery { Name = "Vibrocompactador", Description = "Rana compactadora tipo canguro", Stock = 3, Price = 70000, IsActive = true },
                new Machinery { Name = "Andamio Tubular", Description = "Cuerpo de andamio con tijeras", Stock = 50, Price = 5000, IsActive = true },
                new Machinery { Name = "Taladro Demoledor", Description = "Martillo rompepavimento eléctrico", Stock = 4, Price = 45000, IsActive = true },
                new Machinery { Name = "Pulidora Industrial", Description = "Pulidora de 9 pulgadas", Stock = 6, Price = 20000, IsActive = true }
            );
            await context.SaveChangesAsync();
        }

        // Seed Data for Customers
        if (!context.Customers.Any())
        {
            context.Customers.AddRange(
                new Customer { Name = "Juan Pérez", Document = "10101010", Email = "juan.perez@example.com", Phone = "3001234567" },
                new Customer { Name = "Constructora El Sol", Document = "900123456", Email = "contacto@elsol.com", Phone = "6044445555" },
                new Customer { Name = "María Rodríguez", Document = "20202020", Email = "maria.rod@example.com", Phone = "3109876543" },
                new Customer { Name = "Inversiones ABC", Document = "800987654", Email = "gerencia@abc.com", Phone = "6012223333" },
                new Customer { Name = "Pedro Gómez", Document = "30303030", Email = "pedro.gomez@example.com", Phone = "3205556666" }
            );
            await context.SaveChangesAsync();
        }
    }
}
