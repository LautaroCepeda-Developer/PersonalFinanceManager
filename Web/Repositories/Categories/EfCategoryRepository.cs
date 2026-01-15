using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web.Data;
using Web.Models;

namespace Web.Repositories.Categories
{
    public class EfCategoryRepository(ApplicationDbContext db) : ICategoryRepository
    {
        private readonly ApplicationDbContext _db = db;

        public async Task<IEnumerable<Category>> GetByUserIdAsync(string userId) => await _db.Categories.Where(c => c.UserId == userId && c.IsDeleted == false).ToListAsync();

        public IQueryable<Category> Query() => _db.Categories.AsQueryable();

        public async Task<IEnumerable<Category>> GetSoftDeletedByUserIdAsync(string userId) => await _db.Categories.Where(c => c.UserId == userId && c.IsDeleted).ToListAsync();

        public async Task<Category?> GetByIdAsync(int id) => await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Category>> ListAsync(Expression<Func<Category, bool>>? predicate = null)
        {
            var query = Query();
            if (predicate != null) query = query.Where(predicate);
            return await query.ToListAsync();
        }

        public async Task AddAsync(Category entity)
        {
            _db.Categories.Add(entity);
            await _db.SaveChangesAsync();
        }

        public void Update(Category entity) => _db.Categories.Update(entity);

        public void Remove(Category entity) => _db.Categories.Remove(entity);

        public async Task<bool> HasExpensesAsync(int categoryId)
        {
            return await _db.Expenses.AnyAsync(e => e.CategoryId == categoryId);
        }
        public void SoftDeleteAsync(Category entity)
        {
            entity.IsDeleted = true;
            _db.Categories.Update(entity);
        }

        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();

    }
}
