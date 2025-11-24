using AdminConstruct.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminConstruct.Web.Services
{
    public class RentalValidationService
    {
        private readonly ApplicationDbContext _context;

        public RentalValidationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsAvailable(int machineryId, DateTime start, DateTime end, Guid? excludeRentalId = null)
        {
            var conflictingRentals = await _context.MachineryRentals
                .Where(r => r.MachineryId == machineryId 
                    && r.IsActive 
                    && r.Id != excludeRentalId
                    && (
                        // El nuevo período inicia durante un alquiler existente
                        (start >= r.StartDateTime && start < r.EndDateTime) ||
                        // El nuevo período termina durante un alquiler existente
                        (end > r.StartDateTime && end <= r.EndDateTime) ||
                        // El nuevo período engloba un alquiler existente
                        (start <= r.StartDateTime && end >= r.EndDateTime)
                    ))
                .AnyAsync();

            return !conflictingRentals;
        }
    }
}
