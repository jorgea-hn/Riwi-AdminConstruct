using AdminConstruct.Ryzor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminConstruct.Ryzor.Controllers;

[Authorize(Roles = "Admin")]
public class ExportController : Controller
{
    private readonly ExportService _export;

    public ExportController(ExportService export)
    {
        _export = export;
    }

    [HttpGet]
    public IActionResult ProductsExcel()
    {
        var bytes = _export.ExportProductsExcel();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "productos.xlsx");
    }

    [HttpGet]
    public IActionResult CustomersExcel()
    {
        var bytes = _export.ExportCustomersExcel();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "clientes.xlsx");
    }

    [HttpGet]
    public IActionResult SalesExcel()
    {
        var bytes = _export.ExportSalesExcel();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ventas.xlsx");
    }
}


