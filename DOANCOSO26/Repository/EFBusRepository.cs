using Microsoft.EntityFrameworkCore;
using   DOANCOSO26.Data;
using DOANCOSO26.Models;

namespace DOANCOSO26.Repository
{
    public class EFBusRepository : IBusRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBusRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bus>> GetAllAsync()
        {
            return await _context.Buses.ToListAsync();
        }

        public async Task<Bus> GetByIdAsync(int id)
        {
            return await _context.Buses.FindAsync(id);
        }

        public async Task AddAsync(Bus buses)
        {
            _context.Buses.Add(buses);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bus buses)
        {
            _context.Buses.Update(buses);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var buses = await _context.Buses.FindAsync(id);
            _context.Buses.Remove(buses);
            await _context.SaveChangesAsync();
        }
        public async Task<List<string>> GetAllBusCompanyAsync()
        {
            return await _context.Buses
                                 .Select(br => br.Company)
                                 .Distinct()
                                 .ToListAsync();
        }
    }
}
