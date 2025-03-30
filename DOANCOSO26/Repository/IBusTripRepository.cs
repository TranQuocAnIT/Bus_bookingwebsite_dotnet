using DOANCOSO26.Models;

namespace DOANCOSO26.Repository
{
    public interface IBusTripRepository
    {
        Task<IEnumerable<BusTrip>> GetAllAsync();
        Task<BusTrip> GetByIdAsync(int id);
        Task AddAsync(BusTrip bustrip);
        Task UpdateAsync(BusTrip bustrip);
        Task DeleteAsync(int id);
        Task<IEnumerable<BusTrip>> GetAllByUserIdAsync(string userId);
     
    }
}
