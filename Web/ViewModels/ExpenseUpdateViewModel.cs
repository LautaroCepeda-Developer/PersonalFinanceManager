using Microsoft.AspNetCore.Mvc.Rendering;
using Web.DTOs.Expense;

namespace Web.ViewModels
{
    public class ExpenseUpdateViewModel
    {
        public ExpenseUpdateDTO Expense { get; set; } = new();
        public IEnumerable<SelectListItem> Categories { get; set; } = [];
    }
}
