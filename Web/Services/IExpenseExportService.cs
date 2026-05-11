using Web.DTOs.Expense;

namespace Web.Services
{
    public interface IExpenseExportService
    {
        Task<byte[]> ExportExpensesAsync(ExpenseExportDTO dto);
    }
}
