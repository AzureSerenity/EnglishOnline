using DoAnCoSo.Models;
using DoAnCoSo.Repositories;
using Microsoft.EntityFrameworkCore;

public class EFAnswerRepository : IAnswerRepository
{
    private readonly ApplicationDbContext _context;

    public EFAnswerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Answer>> GetAllAsync()
    {
        return await _context.Answers
                             .Include(a => a.cauHoi)
                             .ToListAsync();
    }

    public async Task<Answer> GetByIdAsync(int id)
    {
        return await _context.Answers
                             .Include(a => a.cauHoi)
                             .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Answer answer)
    {
        _context.Answers.Add(answer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Answer answer)
    {
        _context.Answers.Update(answer);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var answer = await _context.Answers.FindAsync(id);
        if (answer != null)
        {
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Answer>> GetByCauHoiIdAsync(int cauHoiId)
    {
        return await _context.Answers
                             .Where(a => a.CauHoiId == cauHoiId) 
                             .ToListAsync();
    }
    public async Task RemoveRange(IEnumerable<Answer> answers)
    {
        _context.Answers.RemoveRange(answers);
        await _context.SaveChangesAsync();
    }
}
