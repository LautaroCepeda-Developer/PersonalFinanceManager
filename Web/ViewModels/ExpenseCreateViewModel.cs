using Microsoft.AspNetCore.Mvc.Rendering;
using Web.DTOs.Expense;

namespace Web.ViewModels
{
    public class ExpenseCreateViewModel
    {
        public ExpenseCreateDTO Expense { get; set; } = new();
        public IEnumerable<SelectListItem> Categories { get; set; } = [];
    }
}
