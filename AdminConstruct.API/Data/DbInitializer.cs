using AdminConstruct.Web.Models;
using AdminConstruct.Web.Data;

namespace AdminConstruct.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any products.
            if (context.Products.Any())
            {
                return;   // DB has been seeded
            }

            var products = new Product[]
            {
                new Product { Id = Guid.NewGuid(), Name = "Portland Cement Type I", Description = "General use cement for construction.", Price = 25.50m, StockQuantity = 500, ImageUrl = "https://images.unsplash.com/photo-1565793298595-6a879b1d9492?auto=format&fit=crop&q=80&w=300&h=300" },
                new Product { Id = Guid.NewGuid(), Name = "Red Brick 6 Holes", Description = "Ceramic brick for load-bearing walls.", Price = 1.20m, StockQuantity = 10000, ImageUrl = "https://images.unsplash.com/photo-1590074072786-a66914d668f1?auto=format&fit=crop&q=80&w=300&h=300" },
                new Product { Id = Guid.NewGuid(), Name = "Steel Rod 1/2\"", Description = "Corrugated steel for concrete reinforcement.", Price = 35.00m, StockQuantity = 200, ImageUrl = "https://images.unsplash.com/photo-1535063406526-977d2c609b63?auto=format&fit=crop&q=80&w=300&h=300" },
                new Product { Id = Guid.NewGuid(), Name = "Fine Sand (m3)", Description = "Fine sand for finishes and plastering.", Price = 45.00m, StockQuantity = 50, ImageUrl = "https://images.unsplash.com/photo-1621262609955-f86237012d29?auto=format&fit=crop&q=80&w=300&h=300" },
                new Product { Id = Guid.NewGuid(), Name = "Crushed Stone 1/2\" (m3)", Description = "Coarse aggregate for concrete.", Price = 55.00m, StockQuantity = 40, ImageUrl = "https://images.unsplash.com/photo-1525381573678-0e84a51e6e02?auto=format&fit=crop&q=80&w=300&h=300" },
                new Product { Id = Guid.NewGuid(), Name = "White Latex Paint (Gallon)", Description = "High quality washable paint.", Price = 65.00m, StockQuantity = 100, ImageUrl = "https://images.unsplash.com/photo-1589939705384-5185137a7f0f?auto=format&fit=crop&q=80&w=300&h=300" },
                new Product { Id = Guid.NewGuid(), Name = "Floor Ceramic 60x60 (m2)", Description = "Non-slip ceramic for interiors.", Price = 42.00m, StockQuantity = 300, ImageUrl = "https://images.unsplash.com/photo-1620626012053-1c167f7ebc8d?auto=format&fit=crop&q=80&w=300&h=300" },
                new Product { Id = Guid.NewGuid(), Name = "PVC Pipe 4\"", Description = "Pipe for sanitary drainage.", Price = 18.50m, StockQuantity = 150, ImageUrl = "https://images.unsplash.com/photo-1605636762397-59c049d50128?auto=format&fit=crop&q=80&w=300&h=300" },
                new Product { Id = Guid.NewGuid(), Name = "Hand Tools Set", Description = "Basic kit: hammer, screwdrivers, pliers.", Price = 85.00m, StockQuantity = 30, ImageUrl = "https://images.unsplash.com/photo-1581235720704-06d3acfcb36f?auto=format&fit=crop&q=80&w=300&h=300" },
                new Product { Id = Guid.NewGuid(), Name = "Safety Helmet", Description = "Certified industrial protection helmet.", Price = 25.00m, StockQuantity = 80, ImageUrl = "https://images.unsplash.com/photo-1504328345606-18bbc8c9d7d1?auto=format&fit=crop&q=80&w=300&h=300" }
            };

            context.Products.AddRange(products);

            // Look for any machinery.
            if (!context.Machineries.Any())
            {
                var machineries = new Machinery[]
                {
                    new Machinery { Name = "Hydraulic Excavator CAT 320", Description = "Crawler excavator for earthmoving.", Price = 500.00m, Stock = 3, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1582236879854-8c764b815307?auto=format&fit=crop&q=80&w=300&h=300" },
                    new Machinery { Name = "Backhoe Loader JCB 3CX", Description = "Versatile machine for excavation and loading.", Price = 350.00m, Stock = 5, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1504307651254-35680f356dfd?auto=format&fit=crop&q=80&w=300&h=300" },
                    new Machinery { Name = "Concrete Mixer 9p3", Description = "Mixer drum with gasoline engine.", Price = 50.00m, Stock = 10, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1590642916589-592bca10dfbf?auto=format&fit=crop&q=80&w=300&h=300" },
                    new Machinery { Name = "Vibratory Roller Compactor", Description = "For soil and asphalt compaction.", Price = 280.00m, Stock = 2, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1579619639356-946766487140?auto=format&fit=crop&q=80&w=300&h=300" },
                    new Machinery { Name = "Tower Crane", Description = "Crane for lifting loads at height.", Price = 800.00m, Stock = 1, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1535732759880-bbd5c7265e3f?auto=format&fit=crop&q=80&w=300&h=300" },
                    new Machinery { Name = "Industrial Electric Generator", Description = "50KW diesel power plant.", Price = 120.00m, Stock = 4, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1495574386629-281b67277c07?auto=format&fit=crop&q=80&w=300&h=300" },
                    new Machinery { Name = "Multidirectional Scaffolding (Body)", Description = "Modular structure for work at height.", Price = 15.00m, Stock = 100, IsActive = true, ImageUrl = "https://images.unsplash.com/photo-1589939705384-5185137a7f0f?auto=format&fit=crop&q=80&w=300&h=300" } // Reusing generic construction image
                };
                context.Machineries.AddRange(machineries);
            }

            context.SaveChanges();
        }
    }
}
