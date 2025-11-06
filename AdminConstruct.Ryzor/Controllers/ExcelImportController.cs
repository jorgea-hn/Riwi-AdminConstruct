using AdminConstruct.Ryzor.Data;
using AdminConstruct.Ryzor.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AdminConstruct.Ryzor.Controllers;

public class ExcelImportController:Controller
{
    private readonly ApplicationDbContext _context;

        public ExcelImportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mostrar formulario
        public IActionResult ExcelImport()
        {
            return View("~/Views/Admin/ExcelImports/ExcelImport.cshtml");
        }

        // POST: Subir y procesar archivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete("Obsolete")]
        public async Task<IActionResult> Upload(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Log = new List<string> { "No se seleccionó ningún archivo." };
                return View("~/Views/Admin/ExcelImports/ExcelImport.cshtml");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var log = new List<string>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
            {
                ViewBag.Log = new List<string> { "El archivo no contiene hojas." };
                return View("~/Views/Admin/ExcelImports/ExcelImport.cshtml");
            }

            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var values = new List<string>();
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        values.Add(worksheet.Cells[row, col].Text);

                    if (values.Any(v => decimal.TryParse(v, out _)) && values.Any(v => !decimal.TryParse(v, out _)))
                    {
                        var product = new Product
                        {
                            Name = values[0],
                            Price = decimal.TryParse(values[1], out var p) ? p : 0,
                            StockQuantity = int.TryParse(values[2], out var s) ? s : 0,
                            Description = values.ElementAtOrDefault(3)
                        };
                        _context.Products.Add(product);
                    }
                    else if (values.Any(v => v.Contains("@")))
                    {
                        var customer = new Customer
                        {
                            Name = values[0],
                            Email = values[1],
                            Document = values.ElementAtOrDefault(2) ?? "",
                            Phone = values.ElementAtOrDefault(3)
                        };
                        _context.Customers.Add(customer);
                    }
                    else
                    {
                        log.Add($"Fila {row}: no se pudo identificar el tipo de registro");
                    }
                }
                catch (Exception ex)
                {
                    log.Add($"Fila {row}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            ViewBag.Log = log;

            return View("~/Views/Admin/ExcelImports/ExcelImport.cshtml");
        }
    }
