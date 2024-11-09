using DoAnCoSo.Models;
using Microsoft.EntityFrameworkCore;

public class EFCauHoiRepository : ICauHoiRepository
{
    private readonly ApplicationDbContext _context;
    public EFCauHoiRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<CauHoi>> GetAllAsync()
    {
        return await _context.CauHois.ToListAsync();
    }
    public async Task<CauHoi> GetByIdAsync(int id)
    {
        // lấy thông tin kèm theo DeThi
        return await _context.CauHois
               .Include(c => c.DapAns)  // Lấy các đáp án của câu hỏi
               .FirstOrDefaultAsync(c => c.Mach == id);
    }
    public async Task AddAsync(CauHoi cauHoi)
    {
        _context.CauHois.Add(cauHoi);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(CauHoi cauHoi)
    {
        _context.CauHois.Update(cauHoi);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        // Tìm câu hỏi cần xóa
        var cauHoi = await _context.CauHois
            .Include(c => c.DapAns)  // Bao gồm cả đáp án liên kết
            .FirstOrDefaultAsync(c => c.Mach == id);

        if (cauHoi == null)
        {
            throw new KeyNotFoundException("Question not found.");
        }

        // Xóa các đáp án liên kết trước
        if (cauHoi.DapAns != null)
        {
            _context.Answers.RemoveRange(cauHoi.DapAns);
        }

        // Xóa câu hỏi
        _context.CauHois.Remove(cauHoi);
        await _context.SaveChangesAsync();
    }
}