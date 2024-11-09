using Microsoft.EntityFrameworkCore;
using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public class EFLichSuLamBaiRepository : ILichSuLamBaiRepository
    {
        private readonly ApplicationDbContext _context;

        public EFLichSuLamBaiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LichSuLamBai>> GetHistoryByUserIdAsync(string userId)
        {
            return await _context.LichSuLamBais
                .Include(l => l.KetQua)
                .Where(l => l.Id == userId)
                .ToListAsync();
        }

        public async Task AddAsync(LichSuLamBai lichSuLamBai)
        {
            await _context.LichSuLamBais.AddAsync(lichSuLamBai);
            await _context.SaveChangesAsync();
        }
    }

}
