using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface ILichSuLamBaiRepository
    {
        Task<IEnumerable<LichSuLamBai>> GetHistoryByUserIdAsync(string userId);
        Task AddAsync(LichSuLamBai lichSuLamBai);
    }

}
