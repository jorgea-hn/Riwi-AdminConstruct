using AdminConstruct.Razor.Data;
using AdminConstruct.Razor.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AdminConstruct.Razor.Controllers
{
    public class ExcelImportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExcelImportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mostrar formulario de carga
        public IActionResult ExcelImport()
        {
            return View("~/Views/Admin/ExcelImports/ExcelImport.cshtml");
        }

        // POST: Subir y procesar archivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcelImport(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "⚠️ No se seleccionó ningún archivo.";
                return RedirectToAction("ExcelImport");
            }

            // Carpeta donde se guardan los archivos
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Nombre único para evitar sobrescribir archivos
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ⚡ Configurar la licencia de EPPlus 8+ antes de usar ExcelPackage
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            var log = new List<string>();
            int insertedProducts = 0;
            int insertedCustomers = 0;

            // Abrir el archivo Excel
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                TempData["ErrorMessage"] = "⚠️ El archivo no contiene hojas.";
                return RedirectToAction("ExcelImport");
            }

            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var values = new List<string>();
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        values.Add(worksheet.Cells[row, col].Text.Trim());

                    // Detecta si es producto
                    if (values.Any(v => decimal.TryParse(v, out _)) && values.Any(v => !decimal.TryParse(v, out _)))
                    {
                        var product = new Product
                        {
                            Name = values.ElementAtOrDefault(0) ?? "",
                            Price = decimal.TryParse(values.ElementAtOrDefault(1), out var p) ? p : 0,
                            StockQuantity = int.TryParse(values.ElementAtOrDefault(2), out var s) ? s : 0,
                            Description = values.ElementAtOrDefault(3) ?? ""
                        };
                        _context.Products.Add(product);
                        insertedProducts++;
                    }
                    // Detecta si es cliente
                    else if (values.Any(v => v.Contains("@")))
                    {
                        var customer = new Customer
                        {
                            Name = values.ElementAtOrDefault(0) ?? "",
                            Email = values.ElementAtOrDefault(1) ?? "",
                            Document = values.ElementAtOrDefault(2) ?? "",
                            Phone = values.ElementAtOrDefault(3) ?? ""
                        };
                        _context.Customers.Add(customer);
                        insertedCustomers++;
                    }
                    else
                    {
                        log.Add($"Fila {row}: no se pudo identificar el tipo de registro.");
                    }
                }
                catch (Exception ex)
                {
                    log.Add($"Fila {row}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"✅ Archivo cargado correctamente. " +
                                         $"{insertedProducts} productos y {insertedCustomers} clientes fueron importados.";

            // Redirige al Dashboard (ajusta la ruta si no usas área Admin)
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
    }
}
