using DOANCOSO26.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DOANCOSO26.Repository
{
    public interface IDriverregisRepository
    {
        Task<IEnumerable<Driverregis>> GetAllAsync();
        Task<Driverregis> GetByIdAsync(int id);
        Task AddAsync(Driverregis driverregis);
        Task UpdateAsync(Driverregis driverregis);
        Task DeleteAsync(int id);
        Task<IEnumerable<Driverregis>> GetByUserIdAsync(string userId);
    }
}
