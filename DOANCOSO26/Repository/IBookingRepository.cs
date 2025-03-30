using DOANCOSO26.Models;

namespace DOANCOSO26.Repository
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking> GetByIdAsync(int id);
        Task<Booking> AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
        Task<Booking> GetByInvoiceCodeAsync(string invoiceCode);
        Task<IEnumerable<Booking>> GetAllByUserIdAsync(string userId);
        Task<List<Booking>> GetBookingsByTripIdAsync(int tripId);
        Task<int> CountBookingsTodayAsync();
        Task<int> CountAllBookingsAsync();
        Task<double> GetTotalPaidBookingsPriceAsync();
    }
}
