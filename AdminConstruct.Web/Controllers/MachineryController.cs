using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AdminConstruct.Web.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class MachineryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public MachineryController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // GET: Machinery
    public async Task<IActionResult> Index()
    {
        var machineries = await _context.Machineries.ToListAsync();
        return View(machineries);
    }

    // GET: Machinery/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Machinery/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,Stock,Price,IsActive")] Machinery machinery)
    {
        if (ModelState.IsValid)
        {
            _context.Add(machinery);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(machinery);
    }

    // GET: Machinery/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var machinery = await _context.Machineries.FindAsync(id);
        if (machinery == null) return NotFound();
        return View(machinery);
    }

    // POST: Machinery/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Stock,Price,IsActive")] Machinery machinery)
    {
        if (id != machinery.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(machinery);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MachineryExists(machinery.Id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(machinery);
    }

    // GET: Machinery/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var machinery = await _context.Machineries.FirstOrDefaultAsync(m => m.Id == id);
        if (machinery == null) return NotFound();
        return View(machinery);
    }

    // POST: Machinery/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var machinery = await _context.Machineries.FindAsync(id);
        if (machinery != null) _context.Machineries.Remove(machinery);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Machinery/ExportToExcel
    public async Task<IActionResult> ExportToExcel()
    {
        var machineries = await _context.Machineries.ToListAsync();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Maquinaria");

        // Encabezados
        worksheet.Cells[1, 1].Value = "Nombre";
        worksheet.Cells[1, 2].Value = "Stock";
        worksheet.Cells[1, 3].Value = "Precio";
        worksheet.Cells[1, 4].Value = "Estado";

        // Datos
        for (int i = 0; i < machineries.Count; i++)
        {
            worksheet.Cells[i + 2, 1].Value = machineries[i].Name;
            worksheet.Cells[i + 2, 2].Value = machineries[i].Stock;
            worksheet.Cells[i + 2, 3].Value = machineries[i].Price;
            worksheet.Cells[i + 2, 4].Value = machineries[i].IsActive ? "Activo" : "Inactivo";
        }

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Maquinaria.xlsx");
    }

    // GET: Machinery/ExportToPdf
    public async Task<IActionResult> ExportToPdf()
    {
        var machineries = await _context.Machineries.ToListAsync();
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.Header()
                    .Text("Listado de Maquinaria")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Nombre");
                            header.Cell().Text("Stock");
                            header.Cell().Text("Precio");
                            header.Cell().Text("Estado");
                        });

                        foreach (var item in machineries)
                        {
                            table.Cell().Text(item.Name);
                            table.Cell().Text(item.Stock.ToString());
                            table.Cell().Text(item.Price.ToString("C"));
                            table.Cell().Text(item.IsActive ? "Activo" : "Inactivo");
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
            });
        }).GeneratePdf();

        return File(document, "application/pdf", "Maquinaria.pdf");
    }

    private bool MachineryExists(int id)
    {
        return _context.Machineries.Any(e => e.Id == id);
    }
}
