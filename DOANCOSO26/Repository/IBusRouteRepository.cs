using DOANCOSO26.Models;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace DOANCOSO26.Repository
{
    public interface IBusRouteRepository
    {
        Task<IEnumerable<BusRoute>> GetAllAsync();
        Task<BusRoute> GetByIdAsync(int id);
        Task AddAsync(BusRoute BusRoute);
        Task UpdateAsync(BusRoute BusRoute);
        Task DeleteAsync(int id);
        Task<List<string>> GetAllStartLocationsAsync();
        Task<List<string>> GetAllEndLocationsAsync();
    }
}
