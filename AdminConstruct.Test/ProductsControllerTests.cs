using AdminConstruct.API.Controllers;
using AdminConstruct.API.DTOs;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AdminConstruct.Test
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Products")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Martillo", Price = 50 });
                context.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Taladro", Price = 150 });
                await context.SaveChangesAsync();
            }

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
                .Returns(new List<ProductDto> { new ProductDto(), new ProductDto() });

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new ProductsController(context, mockMapper.Object);

                // Act
                var result = await controller.GetAll();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var returnProducts = Assert.IsType<List<ProductDto>>(okResult.Value);
                Assert.Equal(2, returnProducts.Count);
            }
        }
    }
}
