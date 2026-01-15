using AutoMapper;
using Microsoft.Extensions.Localization;
using Web.DTOs;
using Web.DTOs.Category;
using Web.Models;
using Web.Repositories.Categories;

namespace Web.Services
{
    public class CategoryService(ICategoryRepository repo, IMapper mapper, IStringLocalizer<ValidationMessages> localizer) : ICategoryService
    {
        // Helper methods
        private bool UserCategoryExists(int categoryId, string userId, string categoryName) => _repo.Query().Any(c => c.Id != categoryId && c.UserId == userId && c.IsDeleted == false && c.Name.ToLower() == categoryName.ToLower());
        private Category? GetDeletedCategory(int categoryId, string userId, string categoryName)
        {
            return _repo.Query()
                .Where(c => c.Id != categoryId
                    && c.UserId == userId
                    && c.Name.ToLower() == categoryName.ToLower()
                    && c.IsDeleted)
                .FirstOrDefault();
        }

        private readonly ICategoryRepository _repo = repo;
        private readonly IMapper _mapper = mapper;
        private readonly IStringLocalizer<ValidationMessages> _localizer = localizer;
        public async Task<OperationResult> AddCategoryAsync(CategoryCreateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.UserId)) return OperationResult.Fail(_localizer["UserIdMissingInCategory"]);
            if (string.IsNullOrEmpty(dto.Name)) return OperationResult.Fail(_localizer["NameMissingInCategory"]);

            // Check for existing category with the same name for the user
            // If the category is soft-deleted, restore it instead of creating a new one
            var existentCategory = _repo.Query().FirstOrDefault(c => c.UserId == dto.UserId && c.Name.ToLower() == dto.Name.ToLower());
            if (existentCategory is not null && existentCategory.IsDeleted)
            {
                existentCategory.IsDeleted = false;
                _repo.Update(existentCategory);
                await _repo.SaveChangesAsync();

                return OperationResult.Restored(_localizer["DeletedCategoryNameConflict", existentCategory.Name]);
            }

            if (existentCategory is not null)
            {
                return OperationResult.Fail(_localizer["CategoryAlreadyExistsError", dto.Name]);
            }

            Category category = _mapper.Map<CategoryCreateDTO, Category>(dto);

            await _repo.AddAsync(category);
            return OperationResult.Ok();
        }

        public async Task<OperationResult> UpdateCategoryAsync(CategoryDTO category)
        {
            Category? existingCategory = await _repo.GetByIdAsync(category.Id);
            if (existingCategory is null)
                return OperationResult.Fail(_localizer["CategoryNotFoundError", category.Id]);

            if (string.IsNullOrEmpty(category.UserId))
                return OperationResult.Fail(_localizer["UserIdMissingInCategory"]);

            if (string.IsNullOrEmpty(category.Name))
                return OperationResult.Fail(_localizer["NameMissingInCategory"]);

            // Check if a category with the same name already exists for the user
            if (UserCategoryExists(category.Id, category.UserId, category.Name))
                return OperationResult.Fail(_localizer["CategoryAlreadyExistsError", category.Name]);

            // Check if there's a deleted category with the same name
            var deletedCategory = GetDeletedCategory(category.Id, category.UserId, category.Name);
            if (deletedCategory is not null)
            {
                deletedCategory.IsDeleted = false;
                _repo.Update(deletedCategory);
                await _repo.SaveChangesAsync();

                return OperationResult.Restored(
                    _localizer["DeletedCategoryNameConflict", category.Name]
                );
            }

            existingCategory.Name = category.Name;

            _repo.Update(existingCategory);
            await _repo.SaveChangesAsync();
            return OperationResult.Ok();
        }

        public async Task<OperationResult> RestoreCategoryAsync(int categoryID)
        {
            var category = await _repo.GetByIdAsync(categoryID);
            if (category is null)
                return OperationResult.Fail(_localizer["CategoryNotFoundError", categoryID]);
            if (!category.IsDeleted)
                return OperationResult.Fail(_localizer["CategoryNotDeletedError", categoryID]);

            category.IsDeleted = false;
            _repo.Update(category);
            await _repo.SaveChangesAsync();

            return OperationResult.Ok(_localizer["CategoryRestoredSuccess", category.Name]);
        }

        public async Task<IEnumerable<CategoryDTO>> GetUserCategoriesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("UserId must be provided to get user categories.", nameof(userId));
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDTO>>(await _repo.GetByUserIdAsync(userId));
        }

        public async Task<IEnumerable<CategoryDTO>> GetUserSoftDeletedCategoriesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("UserId must be provided to get user categories.", nameof(userId));
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDTO>>(await _repo.GetSoftDeletedByUserIdAsync(userId));
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int categoryId)
        {
            Category? category = await _repo.GetByIdAsync(categoryId);
           
            return category is null ? null : _mapper.Map<Category, CategoryDTO>(category);
        } 

        public async Task<OperationResult> DeleteCategoryByIdAsync(int categoryId)
        {
            Category? category = await _repo.GetByIdAsync(categoryId);
            if (category is null) return OperationResult.Fail(_localizer["CategoryNotFoundError", categoryId]);

            bool hasExpenses = await _repo.HasExpensesAsync(categoryId);

            // Logic deletion if there are associated expenses
            if (hasExpenses)
            {
                _repo.SoftDeleteAsync(category);
                await _repo.SaveChangesAsync();
                return OperationResult.Ok();
            }

            // Physical deletion if there are no associated expenses
            _repo.Remove(category);
            await _repo.SaveChangesAsync();
            return OperationResult.Ok();

        }

    }
}
