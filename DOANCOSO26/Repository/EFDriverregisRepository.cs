using DOANCOSO26.Data;
using DOANCOSO26.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DOANCOSO26.Repository
{
    public class EFDriverregisRepository : IDriverregisRepository
    {
        private readonly ApplicationDbContext _context;

        public EFDriverregisRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Driverregis>> GetAllAsync()
        {
            return await _context.Driverregis
                .Include(d => d.GPLX1Image) // Include related GPLX1Image
                .Include(d => d.GPLX2Image) // Include related GPLX2Image
                .ToListAsync();
        }

        public async Task<Driverregis> GetByIdAsync(int id)
        {
            return await _context.Driverregis
                .Include(d => d.GPLX1Image)
                .Include(d => d.GPLX2Image)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddAsync(Driverregis driverregis)
        {
            _context.Driverregis.Add(driverregis);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Driverregis driverregis)
        {
            _context.Driverregis.Update(driverregis);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Driverregis>> GetByUserIdAsync(string userId)
        {
            return await _context.Driverregis
                .Where(d => d.DriverId == userId)
                .ToListAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var driverregis = await _context.Driverregis.FindAsync(id);
            if (driverregis != null)
            {
                _context.Driverregis.Remove(driverregis);
                await _context.SaveChangesAsync();
            }
        }
    }
}
