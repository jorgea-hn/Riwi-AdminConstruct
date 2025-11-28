using AdminConstruct.API.DTOs;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AdminConstruct.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;

    public CustomersController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: api/customers
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _context.Customers
            .Include(c => c.User)
            .ToListAsync();
        return Ok(_mapper.Map<List<CustomerDto>>(customers));
    }

    // GET: api/customers/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null) return NotFound();

        return Ok(_mapper.Map<CustomerDto>(customer));
    }

    // GET: api/customers/my-profile
    [HttpGet("my-profile")]
    public async Task<IActionResult> GetMyProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Email == user.Email);

        if (customer == null)
        {
            // Create default profile if not exists
            customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = user.UserName ?? "User",
                Email = user.Email!,
                Document = "Pending", // Default value
                UserId = user.Id
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        return Ok(_mapper.Map<CustomerDto>(customer));
    }

    // PUT: api/customers/my-profile
    [HttpPut("my-profile")]
    public async Task<IActionResult> UpdateMyProfile(CustomerDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == user.Email);

        if (customer == null) return NotFound("Customer profile not found");

        customer.Name = dto.Name;
        customer.Document = dto.Document;
        customer.Phone = dto.Phone;

        _context.Entry(customer).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<CustomerDto>(customer));
    }
}