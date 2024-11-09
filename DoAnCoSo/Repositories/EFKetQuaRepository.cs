using Microsoft.EntityFrameworkCore;
using DoAnCoSo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAnCoSo.Repositories
{
    public class EFKetQuaRepository : IKetQuaRepository
    {
        private readonly ApplicationDbContext _context;

        public EFKetQuaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(KetQua ketQua)
        {
            _context.KetQuas.Add(ketQua);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<KetQua>> GetAllAsync()
        {
            return await _context.KetQuas.ToListAsync();
        }

        public async Task<KetQua> GetByIdAsync(int id)
        {
            return await _context.KetQuas.FindAsync(id);
        }

        public async Task UpdateAsync(KetQua ketQua)
        {
            _context.KetQuas.Update(ketQua);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ketQua = await _context.KetQuas.FindAsync(id);
            _context.KetQuas.Remove(ketQua);
            await _context.SaveChangesAsync();
        }
    }
}
