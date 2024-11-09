using DoAnCoSo.Models;

namespace DoAnCoSo.Repositories
{
    public interface IAnswerRepository
    {
        Task AddAsync(Answer answer);
        Task<IEnumerable<Answer>> GetAllAsync();
        Task<Answer> GetByIdAsync(int id);
        Task UpdateAsync(Answer answer);
        Task DeleteAsync(int id);
        Task<IEnumerable<Answer>> GetByCauHoiIdAsync(int cauHoiId);
        Task RemoveRange(IEnumerable<Answer> answers);
    }
}
