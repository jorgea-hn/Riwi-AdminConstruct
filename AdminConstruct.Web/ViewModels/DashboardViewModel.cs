namespace AdminConstruct.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalMachinery { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        
        // Estadísticas de Alquileres
        public int ActiveRentals { get; set; }
        public int CompletedRentals { get; set; }
        public decimal RentalRevenue { get; set; }
        public int OverdueRentals { get; set; }
    }
}
