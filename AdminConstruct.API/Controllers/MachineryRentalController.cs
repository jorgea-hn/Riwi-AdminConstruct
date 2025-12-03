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

            var query = _context.MachineryRentals
                .Include(r => r.Machinery)
                .Include(r => r.Customer)
                .AsQueryable();

            if (!isAdmin)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user!.Email);
                if (customer == null) return Ok(new List<MachineryRentalDto>());
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

            return Ok(_mapper.Map<MachineryRentalDto>(rental));
        }

        // POST: api/machineryrental
        [HttpPost]
        public async Task<IActionResult> Create(CreateMachineryRentalDto dto)
        {
            var machinery = await _context.Machineries.FindAsync(dto.MachineryId);
            if (machinery == null) return BadRequest("Machinery not found");

            if (dto.StartDateTime >= dto.EndDateTime)
            {
                return BadRequest("End date must be after start date");
            }

            // Check availability
            var conflictingRentals = await _context.MachineryRentals
                .Where(r => r.MachineryId == dto.MachineryId && r.IsActive &&
                           ((dto.StartDateTime >= r.StartDateTime && dto.StartDateTime < r.EndDateTime) ||
                            (dto.EndDateTime > r.StartDateTime && dto.EndDateTime <= r.EndDateTime) ||
                            (dto.StartDateTime <= r.StartDateTime && dto.EndDateTime >= r.EndDateTime)))
                .CountAsync();

            if (conflictingRentals >= machinery.Stock)
            {
                return BadRequest("No hay disponibilidad para esta maquinaria en las fechas seleccionadas.");
            }

            // Calculate total amount
            var days = (dto.EndDateTime - dto.StartDateTime).TotalDays;
            if (days < 1) days = 1; // Minimum 1 day
            var totalAmount = (decimal)days * machinery.Price;

            var rental = _mapper.Map<MachineryRental>(dto);
            rental.TotalAmount = totalAmount;
            rental.IsActive = true;

            _context.MachineryRentals.Add(rental);
            await _context.SaveChangesAsync();

            // Send email
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer != null && !string.IsNullOrEmpty(customer.Email))
            {
                try
                {
                    await _emailService.SendEmailAsync(customer.Email, "Rental Confirmation", 
                        $"<h1>Rental Confirmed</h1><p>You have rented {machinery.Name} from {dto.StartDateTime.ToShortDateString()} to {dto.EndDateTime.ToShortDateString()}. Total: ${totalAmount:F2}</p>");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = rental.Id }, _mapper.Map<MachineryRentalDto>(rental));
        }
    }
}
