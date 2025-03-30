using Microsoft.EntityFrameworkCore;
using               DOANCOSO26.Data;
using DOANCOSO26.Models;

namespace DOANCOSO26.Repository
{
    public class EFBusTripRepository: IBusTripRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBusTripRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tương tự như EFProductRepository, nhưng cho BusTrip
        public async Task<IEnumerable<BusTrip>> GetAllAsync()
        {
            return await _context.BusTrips.Include(p => p.Bus).Include(p => p.BusRoute).Include(p => p.Seats).Include(p => p.Driver).Include(p => p.Admin).ToListAsync();
        }

        public async Task<BusTrip> GetByIdAsync(int id)
        {
            return await _context.BusTrips.Include(p => p.Bus).Include(p => p.BusRoute).Include(p => p.Seats).Include(p => p.Driver).Include(p => p.Admin).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(BusTrip bustrip)
        {
            _context.BusTrips.Add(bustrip);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<BusTrip>> GetAllByUserIdAsync(string userId)
        {
            return await _context.BusTrips.Where(b => b.DriverId == userId).ToListAsync();
        }
        public async Task UpdateAsync(BusTrip bustrip)
        {
            _context.BusTrips.Update(bustrip);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var BusTrip = await _context.BusTrips.FindAsync(id);
            _context.BusTrips.Remove(BusTrip);
            await _context.SaveChangesAsync();
        }


       

    }
}
