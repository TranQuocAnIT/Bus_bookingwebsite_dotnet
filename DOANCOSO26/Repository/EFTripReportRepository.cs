using DOANCOSO26.Data;
using DOANCOSO26.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DOANCOSO26.Repository
{
    public class EFTripReportRepository : ITripReportRepository
    {
        private readonly ApplicationDbContext _context;

        public EFTripReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TripReport>> GetAllAsync()
        {
            return await _context.TripReports.Include(p => p.BusTrip.BusRoute).Include(p => p.BusTrip.Bus).Include(p => p.Driver).ToListAsync();
        }

        public async Task<TripReport> GetByIdAsync(int id)
        {
            return await _context.TripReports.Include(p => p.BusTrip.BusRoute).Include(p => p.BusTrip.Bus).Include(p => p.Driver).FirstOrDefaultAsync(tr => tr.Id == id);
        }

        public async Task AddAsync(TripReport tripReport)
        {
            _context.TripReports.Add(tripReport);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TripReport tripReport)
        {
            _context.TripReports.Update(tripReport);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tripReport = await _context.TripReports.FindAsync(id);
            if (tripReport != null)
            {
                _context.TripReports.Remove(tripReport);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<TripReport>> GetTripReportsByBusTripIdAsync(int tripId)
        {

            return await _context.TripReports
                .Where(b => b.BusTripId == tripId)
                .ToListAsync();
        }
        public async Task<double> GetTotalCostAsync()
        {
            return await _context.TripReports
                .SumAsync(tr => (tr.Gascost ?? 0) + (tr.Repaircosts ?? 0) + (tr.Anothercost ?? 0));
        }
    }
}
