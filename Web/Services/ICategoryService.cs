using Web.DTOs;
using Web.DTOs.Category;

namespace Web.Services
{
    public interface ICategoryService
    {
        Task<OperationResult> AddCategoryAsync(CategoryCreateDTO category);
        Task<IEnumerable<CategoryDTO>> GetUserCategoriesAsync(string userId);
        Task<OperationResult> UpdateCategoryAsync(CategoryDTO category);
        Task<CategoryDTO?> GetCategoryByIdAsync(int categoryId);
        Task<OperationResult> DeleteCategoryByIdAsync(int categoryId);
    }
}
