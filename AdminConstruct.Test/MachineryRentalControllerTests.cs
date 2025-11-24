using AdminConstruct.API.Controllers;
using AdminConstruct.API.DTOs;
using AdminConstruct.API.Services;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

namespace AdminConstruct.Test
{
    public class MachineryRentalControllerTests
    {
        [Fact]
        public async Task Create_ReturnsBadRequest_WhenNoStockAvailable()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Rentals_NoStock")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                // Maquinaria con Stock 1
                context.Machineries.Add(new Machinery { Id = 1, Name = "Excavadora", Stock = 1, Price = 100, IsActive = true });
                
                // Alquiler activo para las mismas fechas
                context.MachineryRentals.Add(new MachineryRental 
                { 
                    Id = Guid.NewGuid(), 
                    MachineryId = 1, 
                    CustomerId = Guid.NewGuid(),
                    StartDateTime = DateTime.Today,
                    EndDateTime = DateTime.Today.AddDays(5),
                    IsActive = true
                });
                
                await context.SaveChangesAsync();
            }

            var mockMapper = new Mock<IMapper>();
            var mockUserManager = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);
            var mockEmailService = new Mock<IEmailService>();

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new MachineryRentalController(context, mockMapper.Object, mockUserManager.Object, mockEmailService.Object);

                var dto = new CreateMachineryRentalDto
                {
                    MachineryId = 1,
                    CustomerId = Guid.NewGuid(),
                    StartDateTime = DateTime.Today.AddDays(1), // Solapa con el existente
                    EndDateTime = DateTime.Today.AddDays(3)
                };

                // Act
                var result = await controller.Create(dto);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("No hay disponibilidad para esta maquinaria en las fechas seleccionadas.", badRequestResult.Value);
            }
        }
    }
}
