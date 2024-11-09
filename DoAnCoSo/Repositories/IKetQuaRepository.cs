using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IKetQuaRepository
    {
        Task AddAsync(KetQua ketQua);
        Task<IEnumerable<KetQua>> GetAllAsync();
        Task<KetQua> GetByIdAsync(int id);
        Task UpdateAsync(KetQua ketQua);
        Task DeleteAsync(int id);
    }
}
