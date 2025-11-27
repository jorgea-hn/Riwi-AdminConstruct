using AdminConstruct.API.DTOs;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AdminConstruct.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CustomersController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/customers
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _context.Customers
            .Include(c => c.User)
            .ToListAsync();
        return Ok(_mapper.Map<List<CustomerDto>>(customers));
    }

    // GET: api/customers/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();

        return Ok(_mapper.Map<CustomerDto>(customer));
    }

    // GET: api/customers/my-profile
    [Authorize]
    [HttpGet("my-profile")]
    public async Task<IActionResult> GetMyProfile()
    {
        // DEBUG LOGS
        Console.WriteLine($"[GetMyProfile] Request reached controller");
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"Claim: {claim.Type} - {claim.Value}");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Fallback: try finding "sub" or "id" if NameIdentifier is missing
        if (string.IsNullOrEmpty(userId))
        {
             userId = User.FindFirst("sub")?.Value 
                      ?? User.FindFirst("id")?.Value
                      ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }

        if (string.IsNullOrEmpty(userId)) 
        {
            Console.WriteLine("[GetMyProfile] UserId not found in claims.");
            return Unauthorized();
        }

        Console.WriteLine($"[GetMyProfile] Found UserId: {userId}");

        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
        {
            // Si el usuario existe pero no tiene perfil de cliente, crearlo automáticamente
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return Unauthorized();

            customer = new Customer
            {
                UserId = userId,
                Email = user.Email!,
                Name = user.UserName ?? "Usuario",
                Document = "Pendiente", // Valor por defecto
                Phone = ""
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        var dto = _mapper.Map<CustomerDto>(customer);
        return Ok(dto);
    }

    // PUT: api/customers/my-profile
    [Authorize]
    [HttpPut("my-profile")]
    public async Task<IActionResult> UpdateMyProfile(CustomerDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
            return NotFound("No se encontró el perfil del cliente");

        // Solo permitir actualizar ciertos campos
        customer.Name = dto.Name;
        customer.Document = dto.Document;
        customer.Phone = dto.Phone;
        // Email no se puede cambiar desde aquí

        await _context.SaveChangesAsync();
        return Ok(_mapper.Map<CustomerDto>(customer));
    }

    // PUT: api/customers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        // No permitir cambiar el UserId
        dto.UserId = customer.UserId;
        
        _mapper.Map(dto, customer);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/customers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}