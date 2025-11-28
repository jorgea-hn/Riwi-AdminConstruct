using AdminConstruct.API.DTOs;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AdminConstruct.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly AdminConstruct.API.Services.IEmailService _emailService;
    private readonly UserManager<IdentityUser> _userManager;

    public SalesController(ApplicationDbContext context, IMapper mapper, AdminConstruct.API.Services.IEmailService emailService, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _emailService = emailService;
        _userManager = userManager;
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user!);
        var isAdmin = roles.Contains("Admin");

        var query = _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Details)
            .ThenInclude(d => d.Product)
            .AsQueryable();

        if (!isAdmin)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user!.Email);
            if (customer == null) return Ok(new List<SaleDto>());
            query = query.Where(s => s.CustomerId == customer.Id);
        }

        var sales = await query.ToListAsync();

        return Ok(_mapper.Map<List<SaleDto>>(sales));
    }

    // GET BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sale = await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Details)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null) return NotFound();

        return Ok(_mapper.Map<SaleDto>(sale));
    }

    // CREATE SALE
    [HttpPost]
    public async Task<IActionResult> Create(SaleDto dto)
    {
        var sale = _mapper.Map<Sale>(dto);

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        // Get customer to send email
        var customer = await _context.Customers.FindAsync(sale.CustomerId);
        if (customer != null && !string.IsNullOrEmpty(customer.Email))
        {
            try
            {
                await _emailService.SendEmailAsync(customer.Email, "Purchase Confirmation", $"<h1>Thank you for your purchase!</h1><p>Your order with ID {sale.Id} has been successfully registered.</p>");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        return Ok(_mapper.Map<SaleDto>(sale));
    }
}
