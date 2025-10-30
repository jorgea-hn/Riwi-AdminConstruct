using AdminConstruct.Ryzor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminConstruct.Ryzor.Controllers;

[Authorize(Roles = "Admin")]
public class ImportController : Controller
{
    private readonly ExcelImportService _service;

    public ImportController(ExcelImportService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Sample()
    {
        var bytes = _service.GenerateSampleWorkbook();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "muestra_datos_mixtos.xlsx");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Selecciona un archivo .xlsx");
            return View();
        }
        await using var stream = file.OpenReadStream();
        var result = await _service.ImportAsync(stream);
        return View("Result", result);
    }
}


