using Web.DTOs.Graphics;

namespace Web.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<ExpensesByCategoryDTO> ExpensesByCategory { get; set; } = [];

        public IEnumerable<DailyExpensesDTO> DailyExpenses { get; set; } = [];

        public IEnumerable<MonthlyExpensesDTO> MonthlyExpenses { get; set; } = [];
    }
}
