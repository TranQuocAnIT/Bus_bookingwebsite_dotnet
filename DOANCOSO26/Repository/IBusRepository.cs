using DOANCOSO26.Models;

namespace DOANCOSO26.Repository
{
    public interface IBusRepository
    {
        Task<IEnumerable<Bus>> GetAllAsync();
        Task<Bus> GetByIdAsync(int id);
        Task AddAsync(Bus buses);
        Task UpdateAsync(Bus buses);
        Task DeleteAsync(int id);
        Task<List<string>> GetAllBusCompanyAsync();
    }
}
