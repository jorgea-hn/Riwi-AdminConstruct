using AdminConstruct.API.DTOs;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MachineryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MachineryController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/machinery
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Machineries.AsQueryable();
            var totalCount = await query.CountAsync();

            var machinery = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<MachineryDto>>(machinery);
            return Ok(new PaginatedResult<MachineryDto>(dtos, totalCount, page, pageSize));
        }

        // GET: api/machinery/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var machinery = await _context.Machineries.FindAsync(id);
            if (machinery == null) return NotFound();

            return Ok(_mapper.Map<MachineryDto>(machinery));
        }

        // POST: api/machinery
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateMachineryDto dto)
        {
            var machinery = _mapper.Map<Machinery>(dto);
            machinery.IsActive = true;

            _context.Machineries.Add(machinery);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = machinery.Id }, _mapper.Map<MachineryDto>(machinery));
        }

        // PUT: api/machinery/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateMachineryDto dto)
        {
            var machinery = await _context.Machineries.FindAsync(id);
            if (machinery == null) return NotFound();

            _mapper.Map(dto, machinery);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/machinery/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var machinery = await _context.Machineries.FindAsync(id);
            if (machinery == null) return NotFound();

            _context.Machineries.Remove(machinery);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
