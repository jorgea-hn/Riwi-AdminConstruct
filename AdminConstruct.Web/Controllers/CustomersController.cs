using AdminConstruct.Web.ViewModels;
using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace AdminConstruct.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Admin/Customers
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

        return View(customers);
    }

    // GET: /Admin/Customers/Create
    public IActionResult Create() => View(new CustomerViewModel());

    // POST: /Admin/Customers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var customer = new Customer
        {
            Id = vm.Id == Guid.Empty ? Guid.NewGuid() : vm.Id,
            Name = vm.Name,
            Document = vm.Document,
            Email = vm.Email,
            Phone = vm.Phone
        };

        _context.Add(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Customers/Edit/5
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

        return View(vm);
    }

    // POST: /Admin/Customers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CustomerViewModel vm)
    {
        if (id != vm.Id) return NotFound();
        if (!ModelState.IsValid) return View(vm);

        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        customer.Name = vm.Name;
        customer.Document = vm.Document;
        customer.Email = vm.Email;
        customer.Phone = vm.Phone;

        _context.Update(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Admin/Customers/Details/5
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

        return View(vm);
    }

    // GET: /Admin/Customers/Delete/5
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

        return View(vm);
    }

    // POST: /Admin/Customers/Delete/5
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
