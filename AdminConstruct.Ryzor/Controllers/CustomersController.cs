using AdminConstruct.Ryzor.Data;
using AdminConstruct.Ryzor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.Ryzor.Controllers;

public class CustomersController:Controller
{
     private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTAR + BÚSQUEDA
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.Document.Contains(search));
            }

            var customers = await query.ToListAsync();
            return View("~/Views/Admin/Customers/Clientes.cshtml", customers);
        }

        // CREAR
        public IActionResult Create() => View("~/Views/Admin/Customers/Create.cshtml");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("~/Views/Admin/Customers/Create.cshtml", customer);

                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar el cliente: {ex.Message}");
                return View("~/Views/Admin/Customers/Create.cshtml", customer);
            }
        }

        // EDITAR
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            return View("~/Views/Admin/Customers/Edit.cshtml", customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Customer customer)
        {
            if (id != customer.Id) return NotFound();

            if (!ModelState.IsValid)
                return View("~/Views/Admin/Customers/Edit.cshtml", customer);

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el cliente: {ex.Message}");
                return View("~/Views/Admin/Customers/Edit.cshtml", customer);
            }
        }

        // DETALLES
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return NotFound();

            return View("~/Views/Admin/Customers/Details.cshtml", customer);
        }

        // ELIMINAR
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return NotFound();

            return View("~/Views/Admin/Customers/Delete.cshtml", customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
