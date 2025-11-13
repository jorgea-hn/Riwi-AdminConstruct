using AdminConstruct.Ryzor.ViewModels;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.Web.Controllers
{
    public class CustomersController : Controller
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

            var customers = await query
                .Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Document = c.Document,
                    Email = c.Email,
                    Phone = c.Phone
                })
                .ToListAsync();

            return View("~/Views/Admin/Customers/Clientes.cshtml", customers);
        }

        // CREAR
        public IActionResult Create() => View("~/Views/Admin/Customers/Create.cshtml", new CustomerViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Customers/Create.cshtml", vm);

            var customer = new Customer
            {
                Id = vm.Id == Guid.Empty ? Guid.NewGuid() : vm.Id,
                Name = vm.Name,
                Document = vm.Document,
                Email = vm.Email,
                Phone = vm.Phone
            };

            try
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar el cliente: {ex.Message}");
                return View("~/Views/Admin/Customers/Create.cshtml", vm);
            }
        }

        // EDITAR
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            var vm = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Document = customer.Document,
                Email = customer.Email,
                Phone = customer.Phone
            };

            return View("~/Views/Admin/Customers/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomerViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
                return View("~/Views/Admin/Customers/Edit.cshtml", vm);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            customer.Name = vm.Name;
            customer.Document = vm.Document;
            customer.Email = vm.Email;
            customer.Phone = vm.Phone;

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el cliente: {ex.Message}");
                return View("~/Views/Admin/Customers/Edit.cshtml", vm);
            }
        }

        // DETALLES
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return NotFound();

            var vm = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Document = customer.Document,
                Email = customer.Email,
                Phone = customer.Phone
            };

            return View("~/Views/Admin/Customers/Details.cshtml", vm);
        }

        // ELIMINAR
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return NotFound();

            var vm = new CustomerViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Document = customer.Document,
                Email = customer.Email,
                Phone = customer.Phone
            };

            return View("~/Views/Admin/Customers/Delete.cshtml", vm);
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
}
