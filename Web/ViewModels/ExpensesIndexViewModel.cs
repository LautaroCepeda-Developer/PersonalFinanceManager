using Web.DTOs.Category;
using Web.DTOs.Expense;
using Web.Models;

namespace Web.ViewModels
{
    public class ExpensesIndexViewModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public IEnumerable<ExpenseDTO> Expenses { get; set; } = [];
        public int? CategoryId { get; set; }
        public IEnumerable<CategoryDTO> Categories { get; set; } = [];
        public decimal TotalAmount { get; set;}
        public int Page { get; set; } = 1;
    }
}
