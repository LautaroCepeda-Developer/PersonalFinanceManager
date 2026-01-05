namespace Web.Repositories.Expenses
{
    public interface IExpenseRepository : IRepository<Models.Expense>
    {
        Task<IEnumerable<Models.Expense>> GetByUserIdAsync(string userId, DateTime? from = null, DateTime? to = null, int? categoryId = null);
        Task<decimal> GetTotalByUserAndPeriodAsync(string userId, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<(string Label, decimal Amount)>> GetMonthlyTotalsAsync(string userId, int year);
    }
}
