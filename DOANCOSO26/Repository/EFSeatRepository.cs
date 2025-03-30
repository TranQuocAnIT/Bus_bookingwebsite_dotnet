using Microsoft.EntityFrameworkCore;
using DOANCOSO26.Data;
using DOANCOSO26.Models;
using static    DOANCOSO26.Repository.EFSeatRepository;

namespace DOANCOSO26.Repository
{
    public class EFSeatRepository : ISeatRepository
    {

        private readonly ApplicationDbContext _context;

        public EFSeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Seat> GetSeatsByBusTripId(int busTripId)
        {
            return _context.Seats.Where(s => s.BusTripId == busTripId).ToList();
        }
       
        public async Task<IEnumerable<Seat>> GetAllAsync()
        {
            return await _context.Seats.Include(p => p.BusTrip).ToListAsync();
        }

        public async Task<Seat> GetByIdAsync(int id)
        {
            return await _context.Seats.Include(p => p.BusTrip).FirstOrDefaultAsync(p => p.Id == id);

        }

        public async Task AddAsync(Seat seat)
        {
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();
        }
        public async Task AddRangeAsync(IEnumerable<Seat> seats)
        {
            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }
        public async Task<Seat> GetSeatByBookingIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Seat)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            return booking?.Seat;
        }

    }
}
