using Web.DTOs;
using Web.DTOs.Expense;
using Web.DTOs.Graphics;

namespace Web.Services
{
    public interface IExpenseService
    {
        Task<ExpenseDTO> GetExpenseByIdAsync(int expenseId);
        Task<OperationResult> AddExpenseAsync(ExpenseCreateDTO expense);
        Task<IEnumerable<ExpenseDTO>> GetUserExpensesAsync(string userId, DateTime? from = null, DateTime? to = null, int? categoryId = null);
        Task<decimal> GetTotalAsync(string userId, DateTime? from, DateTime? to);
        Task<IEnumerable<(string Label, decimal Amount)>> GetMonthlyTotalsAsync(string userId, int year);
        Task<IEnumerable<ExpensesByCategoryDTO>> GetCurrentMonthExpensesByCategoryAsync(string userId);
        Task<IEnumerable<DailyExpensesDTO>> GetLast30DaysExpensesAsync(string userId);
        Task<IEnumerable<MonthlyExpensesDTO>> GetLast12MonthsExpensesAsync(string userId);
        Task<OperationResult> UpdateExpenseAsync(ExpenseUpdateDTO dto);
        Task<OperationResult> DeleteExpenseAsync(int expenseId);
    }
}
