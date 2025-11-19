using AdminConstruct.API.DTOs;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SalesController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Details)
            .ThenInclude(d => d.Product)
            .ToListAsync();

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

        return Ok(_mapper.Map<SaleDto>(sale));
    }
}
