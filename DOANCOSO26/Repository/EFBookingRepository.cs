using Microsoft.EntityFrameworkCore;
using   DOANCOSO26.Data;
using   DOANCOSO26.Models;

namespace DOANCOSO26.Repository
{
    public class EFBookingRepository :IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tương tự như EFProductRepository, nhưng cho Booking
        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(p => p.Id == id);

        }
        public async Task<Booking> AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }
       

        // Trong BookingRepository
        public async Task<IEnumerable<Booking>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
        }
        public async Task<Booking> GetByInvoiceCodeAsync(string invoiceCode)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.InvoiceCode == invoiceCode);
        }
        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
        public async Task<int> CountBookingsTodayAsync()
        {
            var today = DateTime.Today;
            return await _context.Bookings
                .Where(b => b.Timebooking.Date == today)
                .CountAsync();
        }
        public async Task<double> GetTotalPaidBookingsPriceAsync()
        {
            return await _context.Bookings
                .Where(b => b.StatusBooking == StatusBooking.Paid)
                .SumAsync(b => b.TotalPrice);
        }
        public async Task<int> CountAllBookingsAsync()
        {
            return await _context.Bookings.CountAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Booking>> GetBookingsByTripIdAsync(int tripId)
        {
            return await _context.Bookings
                .Include(b => b.PickupStop)
                .Include(b => b.DropOffStop)
                .Where(b => b.TripId == tripId)
                .ToListAsync();
        }

    }
}
