using Microsoft.EntityFrameworkCore;
using DoAnCoSo.Models;
using DoAnCoSo.Repositories;

public class EFBaiThiRepository : IBaiThiRepository
{
    private readonly ApplicationDbContext _context;
    public EFBaiThiRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BaiThi>> GetAllAsync()
    {
        return await _context.BaiThis
            .Include(b => b.BaiThi_CauHois)
            .ToListAsync();
    }

    public async Task<BaiThi> GetByIdAsync(int id)
    {
        // lấy thông tin kèm theo DeThi
        return await _context.BaiThis.Include(p => p.BaiThi_CauHois).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(BaiThi baiThi)
    {
        _context.BaiThis.Add(baiThi);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(BaiThi baiThi)
    {
        _context.BaiThis.Update(baiThi);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var baiThi = await _context.BaiThis.FindAsync(id); 
        if (baiThi != null) 
        { 
            _context.BaiThis.Remove(baiThi); 
            await _context.SaveChangesAsync(); 
        }
    }
}