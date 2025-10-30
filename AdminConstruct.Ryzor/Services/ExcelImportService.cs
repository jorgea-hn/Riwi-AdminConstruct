using AdminConstruct.Ryzor.Data;
using AdminConstruct.Ryzor.Models;
using OfficeOpenXml;

namespace AdminConstruct.Ryzor.Services;

public class ExcelImportResult
{
    public int ProductsUpserted { get; set; }
    public int CustomersUpserted { get; set; }
    public int SalesCreated { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ExcelImportService
{
    private readonly ApplicationDbContext _db;

    public ExcelImportService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ExcelImportResult> ImportAsync(Stream excelStream)
    {
        var result = new ExcelImportResult();

        using var package = new ExcelPackage(excelStream);
        var sheet = package.Workbook.Worksheets.FirstOrDefault();
        if (sheet == null)
        {
            result.Errors.Add("No se encontró hoja en el archivo.");
            return result;
        }

        // Map headers flexibles
        var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var colCount = sheet.Dimension?.End.Column ?? 0;
        for (int c = 1; c <= colCount; c++)
        {
            var h = sheet.Cells[1, c].GetValue<string>()?.Trim();
            if (!string.IsNullOrEmpty(h) && !headers.ContainsKey(h)) headers[h] = c;
        }

        // Helper para obtener valor por posibles nombres de columna
        string? GetString(int row, params string[] names)
        {
            foreach (var n in names)
            {
                if (headers.TryGetValue(n, out var idx))
                {
                    var v = sheet.Cells[row, idx].GetValue<string>();
                    if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
                }
            }
            return null;
        }

        decimal? GetDecimal(int row, params string[] names)
        {
            var s = GetString(row, names);
            if (decimal.TryParse(s, out var d)) return d;
            return null;
        }

        int? GetInt(int row, params string[] names)
        {
            var s = GetString(row, names);
            if (int.TryParse(s, out var i)) return i;
            return null;
        }

        DateTime? GetDate(int row, params string[] names)
        {
            var s = GetString(row, names);
            if (DateTime.TryParse(s, out var dt)) return dt;
            return null;
        }

        var endRow = sheet.Dimension?.End.Row ?? 0;
        for (int r = 2; r <= endRow; r++)
        {
            try
            {
                var productName = GetString(r, "Producto", "Product", "NombreProducto");
                var price = GetDecimal(r, "Precio", "Price");
                var stock = GetInt(r, "Stock", "CantidadStock");

                var customerName = GetString(r, "Cliente", "Customer", "NombreCliente");
                var customerDoc = GetString(r, "Documento", "Document", "NroDocumento");
                var customerEmail = GetString(r, "Email", "Correo");
                var customerPhone = GetString(r, "Telefono", "Phone");

                var saleDate = GetDate(r, "Fecha", "SaleDate");
                var quantity = GetInt(r, "Cantidad", "Quantity");
                var unitPrice = GetDecimal(r, "UnitPrice", "PrecioUnitario");

                var hasProductData = !string.IsNullOrWhiteSpace(productName) && price.HasValue;
                var hasCustomerData = !string.IsNullOrWhiteSpace(customerName) && !string.IsNullOrWhiteSpace(customerDoc);
                var hasSaleData = saleDate.HasValue && !string.IsNullOrWhiteSpace(productName) && !string.IsNullOrWhiteSpace(customerDoc) && quantity.HasValue && unitPrice.HasValue;

                if (!hasProductData && !hasCustomerData && !hasSaleData)
                {
                    // fila vacía o irrelevante
                    continue;
                }

                // Upsert Product
                Product? product = null;
                if (hasProductData)
                {
                    product = _db.Products.FirstOrDefault(p => p.Name.ToLower() == productName!.ToLower());
                    if (product == null)
                    {
                        product = new Product { Name = productName!, Price = price!.Value, StockQuantity = stock ?? 0 };
                        _db.Products.Add(product);
                    }
                    else
                    {
                        product.Price = price!.Value;
                        if (stock.HasValue) product.StockQuantity = stock.Value;
                    }
                    result.ProductsUpserted++;
                }

                // Upsert Customer
                Customer? customer = null;
                if (hasCustomerData)
                {
                    customer = _db.Customers.FirstOrDefault(c => c.Document.ToLower() == customerDoc!.ToLower());
                    if (customer == null)
                    {
                        customer = new Customer { Name = customerName!, Document = customerDoc!, Email = customerEmail ?? string.Empty, Phone = customerPhone };
                        _db.Customers.Add(customer);
                    }
                    else
                    {
                        customer.Name = customerName!;
                        if (!string.IsNullOrWhiteSpace(customerEmail)) customer.Email = customerEmail!;
                        if (!string.IsNullOrWhiteSpace(customerPhone)) customer.Phone = customerPhone!;
                    }
                    result.CustomersUpserted++;
                }

                // Create Sale and Detail per row if present
                if (hasSaleData)
                {
                    // Ensure related entities exist
                    customer ??= _db.Customers.FirstOrDefault(c => c.Document.ToLower() == customerDoc!.ToLower());
                    if (customer == null)
                    {
                        result.Errors.Add($"Fila {r}: Cliente con documento '{customerDoc}' no existe ni pudo crearse.");
                        continue;
                    }
                    product ??= _db.Products.FirstOrDefault(p => p.Name.ToLower() == productName!.ToLower());
                    if (product == null)
                    {
                        result.Errors.Add($"Fila {r}: Producto '{productName}' no existe ni pudo crearse.");
                        continue;
                    }

                    var sale = new Sale
                    {
                        CustomerId = customer.Id,
                        Date = saleDate!.Value
                    };
                    _db.Sales.Add(sale);

                    var detail = new SaleDetail
                    {
                        SaleId = sale.Id,
                        ProductId = product.Id,
                        Quantity = quantity!.Value,
                        UnitPrice = unitPrice!.Value
                    };
                    _db.SaleDetails.Add(detail);
                    result.SalesCreated++;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Fila {r}: {ex.Message}");
            }
        }

        await _db.SaveChangesAsync();
        return result;
    }

    public byte[] GenerateSampleWorkbook()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Datos");
        // Cabeceras mixtas y flexibles
        var headers = new[] { "Producto", "Precio", "Stock", "Cliente", "Documento", "Email", "Telefono", "Fecha", "Cantidad", "UnitPrice" };
        for (int i = 0; i < headers.Length; i++) ws.Cells[1, i + 1].Value = headers[i];

        // Filas mezcladas: producto solo, cliente solo, venta (requiere producto+cliente)
        ws.Cells[2, 1].Value = "Cemento Gris"; ws.Cells[2, 2].Value = 32000; ws.Cells[2, 3].Value = 50; // producto
        ws.Cells[3, 4].Value = "Juan Pérez"; ws.Cells[3, 5].Value = "CC123"; ws.Cells[3, 6].Value = "juan@example.com"; ws.Cells[3, 7].Value = "+57 3001112233"; // cliente
        ws.Cells[4, 1].Value = "Cemento Gris"; ws.Cells[4, 4].Value = "Juan Pérez"; ws.Cells[4, 5].Value = "CC123"; ws.Cells[4, 8].Value = DateTime.Today.ToString("yyyy-MM-dd"); ws.Cells[4, 9].Value = 10; ws.Cells[4, 10].Value = 32000; // venta

        ws.Cells[1, 1, 1, headers.Length].AutoFilter = true;
        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }
}


