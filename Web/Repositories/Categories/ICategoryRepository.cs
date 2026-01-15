using Web.Models;

namespace Web.Repositories.Categories
{
    public interface ICategoryRepository : IRepository<Models.Category>
    {
        Task<IEnumerable<Models.Category>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Models.Category>> GetSoftDeletedByUserIdAsync(string userId);
        void SoftDeleteAsync(Category category);
        Task<bool> HasExpensesAsync(int categoryId);

    }
}
