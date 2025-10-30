using AdminConstruct.Ryzor.Data;
using OfficeOpenXml;

namespace AdminConstruct.Ryzor.Services;

public class ExportService
{
    private readonly ApplicationDbContext _db;

    public ExportService(ApplicationDbContext db)
    {
        _db = db;
    }

    public byte[] ExportProductsExcel()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Productos");
        ws.Cells[1, 1].Value = "Nombre";
        ws.Cells[1, 2].Value = "Precio";
        ws.Cells[1, 3].Value = "Stock";

        var data = _db.Products.OrderBy(p => p.Name).ToList();
        int r = 2;
        foreach (var p in data)
        {
            ws.Cells[r, 1].Value = p.Name;
            ws.Cells[r, 2].Value = p.Price;
            ws.Cells[r, 3].Value = p.StockQuantity;
            r++;
        }
        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public byte[] ExportCustomersExcel()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Clientes");
        ws.Cells[1, 1].Value = "Nombre";
        ws.Cells[1, 2].Value = "Documento";
        ws.Cells[1, 3].Value = "Email";
        ws.Cells[1, 4].Value = "Telefono";

        var data = _db.Customers.OrderBy(c => c.Name).ToList();
        int r = 2;
        foreach (var c in data)
        {
            ws.Cells[r, 1].Value = c.Name;
            ws.Cells[r, 2].Value = c.Document;
            ws.Cells[r, 3].Value = c.Email;
            ws.Cells[r, 4].Value = c.Phone;
            r++;
        }
        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    public byte[] ExportSalesExcel()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Ventas");
        ws.Cells[1, 1].Value = "Fecha";
        ws.Cells[1, 2].Value = "Cliente";
        ws.Cells[1, 3].Value = "Producto";
        ws.Cells[1, 4].Value = "Cantidad";
        ws.Cells[1, 5].Value = "UnitPrice";

        var data = (from d in _db.SaleDetails
                    join s in _db.Sales on d.SaleId equals s.Id
                    join c in _db.Customers on s.CustomerId equals c.Id
                    join p in _db.Products on d.ProductId equals p.Id
                    orderby s.Date descending
                    select new { s.Date, Customer = c.Name, Product = p.Name, d.Quantity, d.UnitPrice }).ToList();

        int r = 2;
        foreach (var v in data)
        {
            ws.Cells[r, 1].Value = v.Date;
            ws.Cells[r, 2].Value = v.Customer;
            ws.Cells[r, 3].Value = v.Product;
            ws.Cells[r, 4].Value = v.Quantity;
            ws.Cells[r, 5].Value = v.UnitPrice;
            r++;
        }
        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}


