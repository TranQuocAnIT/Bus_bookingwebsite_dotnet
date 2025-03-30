using DOANCOSO26.Models;

namespace DOANCOSO26.Repository
{
    public interface ITripReportRepository
    {
        Task<IEnumerable<TripReport>> GetAllAsync();
        Task<TripReport> GetByIdAsync(int id);
        Task AddAsync(TripReport tripReport);
        Task UpdateAsync(TripReport tripReport);
        Task DeleteAsync(int id); 
        Task<double> GetTotalCostAsync();
        Task<List<TripReport>> GetTripReportsByBusTripIdAsync(int tripId);
    }
}
