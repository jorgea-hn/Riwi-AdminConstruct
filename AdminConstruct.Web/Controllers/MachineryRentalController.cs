using AdminConstruct.Web.Data;
using AdminConstruct.Web.Models;
using AdminConstruct.Web.Services;
using AdminConstruct.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using OfficeOpenXml;

namespace AdminConstruct.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MachineryRentalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RentalValidationService _validationService;

        public MachineryRentalController(ApplicationDbContext context, RentalValidationService validationService)
        {
            _context = context;
            _validationService = validationService;
        }

        // GET: MachineryRental
        public async Task<IActionResult> Index()
        {
            var rentals = await _context.MachineryRentals
                .Include(m => m.Machinery)
                .Include(m => m.Customer)
                .OrderByDescending(m => m.StartDateTime)
                .ToListAsync();
            return View("~/Views/Admin/MachineryRental/Index.cshtml", rentals);
        }

        // GET: MachineryRental/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineryRental = await _context.MachineryRentals
                .Include(m => m.Machinery)
                .Include(m => m.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (machineryRental == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/MachineryRental/Details.cshtml", machineryRental);
        }

        // GET: MachineryRental/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            ViewData["MachineryId"] = new SelectList(_context.Machineries.Where(m => m.IsActive && m.Stock > 0), "Id", "Name");
            return View("~/Views/Admin/MachineryRental/Create.cshtml");
        }

        // POST: MachineryRental/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MachineryId,CustomerId,StartDateTime,EndDateTime,PricePerDay,Notes")] MachineryRental machineryRental)
        {
            if (ModelState.IsValid)
            {
                // Validar fechas
                if (machineryRental.StartDateTime >= machineryRental.EndDateTime)
                {
                    ModelState.AddModelError("EndDateTime", "La fecha de fin debe ser posterior a la fecha de inicio.");
                }

                // Validar disponibilidad
                if (!await _validationService.IsAvailable(machineryRental.MachineryId, machineryRental.StartDateTime, machineryRental.EndDateTime))
                {
                    ModelState.AddModelError("", "La maquinaria seleccionada no está disponible en el horario especificado.");
                }

                if (ModelState.IsValid)
                {
                    // Calcular total
                    var days = (machineryRental.EndDateTime - machineryRental.StartDateTime).TotalDays;
                    // Redondear hacia arriba para cobrar días completos o fracciones como día
                    var billingDays = Math.Ceiling(days);
                    if (billingDays < 1) billingDays = 1;
                    
                    machineryRental.TotalAmount = machineryRental.PricePerDay * (decimal)billingDays;
                    machineryRental.Id = Guid.NewGuid();
                    machineryRental.IsActive = true;
                    machineryRental.CreatedAt = DateTime.Now;

                    _context.Add(machineryRental);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", machineryRental.CustomerId);
            ViewData["MachineryId"] = new SelectList(_context.Machineries.Where(m => m.IsActive && m.Stock > 0), "Id", "Name", machineryRental.MachineryId);
            return View("~/Views/Admin/MachineryRental/Create.cshtml", machineryRental);
        }

        // GET: MachineryRental/Return/5
        public async Task<IActionResult> Return(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineryRental = await _context.MachineryRentals
                .Include(m => m.Machinery)
                .Include(m => m.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (machineryRental == null)
            {
                return NotFound();
            }

            if (!machineryRental.IsActive)
            {
                return RedirectToAction(nameof(Details), new { id = machineryRental.Id });
            }

            return View("~/Views/Admin/MachineryRental/Return.cshtml", machineryRental);
        }

        // POST: MachineryRental/Return/5
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(Guid id)
        {
            var machineryRental = await _context.MachineryRentals.FindAsync(id);
            if (machineryRental != null)
            {
                machineryRental.IsActive = false;
                machineryRental.ActualReturnDate = DateTime.Now;
                _context.Update(machineryRental);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        
        // API para obtener precio de maquinaria
        [HttpGet]
        public async Task<IActionResult> GetMachineryPrice(int id)
        {
            var machinery = await _context.Machineries.FindAsync(id);
            if (machinery == null) return NotFound();
            return Json(new { price = machinery.Price });
        }

        // Exportar a PDF
        public async Task<IActionResult> ExportToPDF()
        {
            var rentals = await _context.MachineryRentals
                .Include(m => m.Machinery)
                .Include(m => m.Customer)
                .OrderByDescending(m => m.StartDateTime)
                .ToListAsync();

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(QuestPDF.Helpers.PageSizes.A4);
                    page.Margin(2, QuestPDF.Infrastructure.Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Text("Reporte de Alquileres de Maquinaria")
                        .SemiBold().FontSize(20).FontColor(QuestPDF.Helpers.Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, QuestPDF.Infrastructure.Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Estado");
                                header.Cell().Element(CellStyle).Text("Maquinaria");
                                header.Cell().Element(CellStyle).Text("Cliente");
                                header.Cell().Element(CellStyle).Text("Inicio");
                                header.Cell().Element(CellStyle).Text("Fin");
                                header.Cell().Element(CellStyle).Text("Total");

                                static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Medium);
                                }
                            });

                            foreach (var item in rentals)
                            {
                                table.Cell().Element(CellStyle).Text(item.IsActive ? "Activo" : "Finalizado");
                                table.Cell().Element(CellStyle).Text(item.Machinery.Name);
                                table.Cell().Element(CellStyle).Text(item.Customer.Name);
                                table.Cell().Element(CellStyle).Text(item.StartDateTime.ToString("dd/MM/yyyy"));
                                table.Cell().Element(CellStyle).Text(item.EndDateTime.ToString("dd/MM/yyyy"));
                                table.Cell().Element(CellStyle).Text(item.TotalAmount.ToString("C"));

                                static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
                                {
                                    return container.PaddingVertical(5).BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2);
                                }
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
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"Reporte_Alquileres_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // Exportar a Excel
        public async Task<IActionResult> ExportToExcel()
        {
            var rentals = await _context.MachineryRentals
                .Include(m => m.Machinery)
                .Include(m => m.Customer)
                .OrderByDescending(m => m.StartDateTime)
                .ToListAsync();

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Alquileres");

                // Encabezados
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Estado";
                worksheet.Cells[1, 3].Value = "Maquinaria";
                worksheet.Cells[1, 4].Value = "Cliente";
                worksheet.Cells[1, 5].Value = "Fecha Inicio";
                worksheet.Cells[1, 6].Value = "Fecha Fin";
                worksheet.Cells[1, 7].Value = "Total";

                // Estilo encabezados
                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Datos
                int row = 2;
                foreach (var item in rentals)
                {
                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.IsActive ? "Activo" : "Finalizado";
                    worksheet.Cells[row, 3].Value = item.Machinery.Name;
                    worksheet.Cells[row, 4].Value = item.Customer.Name;
                    worksheet.Cells[row, 5].Value = item.StartDateTime.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 6].Value = item.EndDateTime.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 7].Value = item.TotalAmount;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Reporte_Alquileres_{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }
    }
}
