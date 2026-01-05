using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using Web.Data;
using Web.Models;

namespace Web.Repositories.Expenses
{
    public class EfExpenseRepository(ApplicationDbContext db) : IExpenseRepository
    {
        private readonly ApplicationDbContext _db = db;

        public IQueryable<Expense> Query() => _db.Expenses.AsQueryable();
        public async Task AddAsync(Expense entity) => await _db.Expenses.AddAsync(entity);
        public void Update(Expense entity) => _db.Expenses.Update(entity);
        public void Remove(Expense entity) => _db.Expenses.Remove(entity);
        public async Task<Expense?> GetByIdAsync(int id) => await _db.Expenses.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);

        public async Task<IEnumerable<Expense>> ListAsync(Expression<Func<Expense, bool>>? predicate = null)
        {
            var query = _db.Expenses.Include(e => e.Category).AsQueryable();
            if (predicate != null) query = query.Where(predicate);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetByUserIdAsync(string userId, DateTime? from = null, DateTime? to = null, int? categoryId = null)
        {
            var query = _db.Expenses.Include(e => e.Category).Where(e => e.UserId == userId).AsQueryable();

            if (from.HasValue)
            {
                var fromLocal = DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Local);
                var fromUtc = fromLocal.ToUniversalTime();

                query = query.Where(e => e.Date >= fromUtc);
            }

            if (to.HasValue)
            {
                var toLocal = DateTime.SpecifyKind(to.Value.Date, DateTimeKind.Local);
                var toUtc = toLocal.ToUniversalTime();

                query = query.Where(e => e.Date <= toUtc);
            }

            if (categoryId.HasValue) query = query.Where(e => e.CategoryId == categoryId);

            return await query.OrderByDescending(e => e.Date).ToListAsync();
        }

        public async Task<decimal> GetTotalByUserAndPeriodAsync(string userId, DateTime? from, DateTime? to)
        {
            return await _db.Expenses.Where(e => e.UserId == userId && e.Date >= from && e.Date <= to).SumAsync(e => (decimal?)e.Amount) ?? 0m;
        }

        public async Task<IEnumerable<(string Label, decimal Amount)>> GetMonthlyTotalsAsync(string userId, int year)
        {
            var query = await _db.Expenses.Where(e => e.UserId == userId && e.Date.Year == year)
                .GroupBy(e => e.Date.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(x => x.Amount) })
                .OrderBy(x => x.Month)
                .ToListAsync();

            return query.Select(x => (Label: CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Month), Amount: x.Total));
        }

        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
