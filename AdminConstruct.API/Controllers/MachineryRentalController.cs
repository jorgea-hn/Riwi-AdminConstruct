using AdminConstruct.API.DTOs;
using AdminConstruct.API.Services;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AdminConstruct.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MachineryRentalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;

        public MachineryRentalController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: api/machineryrental
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user!);
            var isAdmin = roles.Contains("Admin");

            IQueryable<MachineryRental> query = _context.MachineryRentals
                .Include(r => r.Machinery)
                .Include(r => r.Customer);

            if (!isAdmin)
            {
                // Si es cliente, filtrar por su ID (asumiendo que CustomerId en Rental se relaciona con el usuario logueado)
                // Nota: Aquí hay un desafío. IdentityUser.Id es string, Customer.Id es Guid.
                // Debemos buscar el Customer asociado al email del usuario logueado.
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user!.Email);
                if (customer == null) return Ok(new List<MachineryRentalDto>()); // No tiene perfil de cliente

                query = query.Where(r => r.CustomerId == customer.Id);
            }

            var rentals = await query.ToListAsync();
            return Ok(_mapper.Map<List<MachineryRentalDto>>(rentals));
        }

        // GET: api/machineryrental/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var rental = await _context.MachineryRentals
                .Include(r => r.Machinery)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null) return NotFound();

            // Validar permisos
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user!);
            if (!roles.Contains("Admin"))
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user!.Email);
                if (customer == null || rental.CustomerId != customer.Id)
                    return Forbid();
            }

            return Ok(_mapper.Map<MachineryRentalDto>(rental));
        }

        // POST: api/machineryrental
        [HttpPost]
        public async Task<IActionResult> Create(CreateMachineryRentalDto dto)
        {
            // 1. Validar fechas
            if (dto.EndDateTime <= dto.StartDateTime)
                return BadRequest("La fecha de fin debe ser posterior a la fecha de inicio.");

            // 2. Validar Maquinaria
            var machinery = await _context.Machineries.FindAsync(dto.MachineryId);
            if (machinery == null) return BadRequest("La maquinaria especificada no existe.");

            // 3. Validar Disponibilidad
            // Contar alquileres activos que se solapen con el rango solicitado
            var activeRentalsCount = await _context.MachineryRentals
                .Where(r => r.MachineryId == dto.MachineryId && r.IsActive)
                .Where(r => r.StartDateTime < dto.EndDateTime && r.EndDateTime > dto.StartDateTime)
                .CountAsync();

            if (activeRentalsCount >= machinery.Stock)
                return BadRequest("No hay disponibilidad para esta maquinaria en las fechas seleccionadas.");

            // 4. Crear Entidad
            var rental = _mapper.Map<MachineryRental>(dto);
            
            // Calcular total
            var days = (dto.EndDateTime - dto.StartDateTime).Days;
            if (days == 0) days = 1; // Mínimo 1 día
            rental.PricePerDay = machinery.Price;
            rental.TotalAmount = rental.PricePerDay * days;
            rental.IsActive = true;

            _context.MachineryRentals.Add(rental);
            await _context.SaveChangesAsync();

            // 5. Enviar Correo
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer != null && !string.IsNullOrEmpty(customer.Email))
            {
                try
                {
                    await _emailService.SendEmailAsync(customer.Email, "Confirmación de Alquiler", 
                        $"<h1>¡Alquiler Confirmado!</h1>" +
                        $"<p>Has alquilado: <strong>{machinery.Name}</strong></p>" +
                        $"<p>Desde: {rental.StartDateTime:dd/MM/yyyy}</p>" +
                        $"<p>Hasta: {rental.EndDateTime:dd/MM/yyyy}</p>" +
                        $"<p>Total: {rental.TotalAmount:C}</p>");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error enviando correo: {ex.Message}");
                }
            }

            // Recargar con relaciones para devolver DTO completo
            await _context.Entry(rental).Reference(r => r.Machinery).LoadAsync();
            await _context.Entry(rental).Reference(r => r.Customer).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = rental.Id }, _mapper.Map<MachineryRentalDto>(rental));
        }
    }
}
