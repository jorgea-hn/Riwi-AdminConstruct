using AdminConstruct.Web.Controllers;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AdminConstruct.Test.Web
{
    public class MachineryControllerWebTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithMachinery()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            context.Machineries.AddRange(
                new Machinery { Id = 1, Name = "Excavadora", Price = 50000, Stock = 5, IsActive = true },
                new Machinery { Id = 2, Name = "Retroexcavadora", Price = 60000, Stock = 3, IsActive = true }
            );
            await context.SaveChangesAsync();

            var mockMapper = new Mock<IMapper>();
            var controller = new MachineryController(context, mockMapper.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Machinery>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Create_Post_AddsMachineryAndRedirects()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var mockMapper = new Mock<IMapper>();
            var controller = new MachineryController(context, mockMapper.Object);
            var newMachinery = new Machinery
            {
                Name = "Grúa",
                Price = 100000,
                Stock = 2,
                Description = "Grúa torre",
                IsActive = true
            };

            // Act
            var result = await controller.Create(newMachinery);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(1, await context.Machineries.CountAsync());
        }

        [Fact]
        public async Task Edit_Post_UpdatesMachinery()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var machinery = new Machinery
            {
                Id = 1,
                Name = "Excavadora",
                Price = 50000,
                Stock = 5,
                IsActive = true
            };
            context.Machineries.Add(machinery);
            await context.SaveChangesAsync();

            var mockMapper = new Mock<IMapper>();
            var controller = new MachineryController(context, mockMapper.Object);
            var updatedMachinery = new Machinery
            {
                Id = 1,
                Name = "Excavadora Hidráulica",
                Price = 55000,
                Stock = 6,
                IsActive = true
            };

            // Act
            var result = await controller.Edit(1, updatedMachinery);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var dbMachinery = await context.Machineries.FindAsync(1);
            Assert.Equal("Excavadora Hidráulica", dbMachinery!.Name);
            Assert.Equal(55000, dbMachinery.Price);
        }

        [Fact]
        public async Task Delete_RemovesMachinery()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var machinery = new Machinery
            {
                Id = 1,
                Name = "Excavadora",
                Price = 50000,
                Stock = 5,
                IsActive = true
            };
            context.Machineries.Add(machinery);
            await context.SaveChangesAsync();

            var mockMapper = new Mock<IMapper>();
            var controller = new MachineryController(context, mockMapper.Object);

            // Act
            var result = await controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(0, await context.Machineries.CountAsync());
        }
    }
}
