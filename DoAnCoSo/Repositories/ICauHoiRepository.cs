using DoAnCoSo.Models;

namespace DoAnCoSo.Models
{
    public interface ICauHoiRepository
    {
        Task<IEnumerable<CauHoi>> GetAllAsync();
        Task<CauHoi> GetByIdAsync(int id);
        Task AddAsync(CauHoi cauHoi);
        Task UpdateAsync(CauHoi cauHoi);
        Task DeleteAsync(int id);

    }
}
