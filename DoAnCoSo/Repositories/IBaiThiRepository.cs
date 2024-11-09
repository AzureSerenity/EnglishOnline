using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IBaiThiRepository
    {
        Task<IEnumerable<BaiThi>> GetAllAsync();
        Task<BaiThi> GetByIdAsync(int id);
        Task AddAsync(BaiThi baiThi);
        Task UpdateAsync(BaiThi baiThi);
        Task DeleteAsync(int id);
    }
}
