using AdminConstruct.Web.Controllers;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AdminConstruct.Test.Web
{
    public class ProductsControllerWebTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithProducts()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            context.Products.AddRange(
                new Product { Id = Guid.NewGuid(), Name = "Cemento", Price = 25000, StockQuantity = 100 },
                new Product { Id = Guid.NewGuid(), Name = "Arena", Price = 15000, StockQuantity = 50 }
            );
            await context.SaveChangesAsync();

            var mockEnv = new Mock<IWebHostEnvironment>();
            var controller = new ProductsController(context, mockEnv.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesProduct()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Cemento",
                Price = 25000,
                StockQuantity = 100
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var mockEnv = new Mock<IWebHostEnvironment>();
            var controller = new ProductsController(context, mockEnv.Object);

            // Act
            var result = await controller.DeleteConfirmed(productId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(0, await context.Products.CountAsync());
        }
    }
}
