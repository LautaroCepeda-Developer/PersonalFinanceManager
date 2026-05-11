using Microsoft.AspNetCore.Mvc.Rendering;
namespace Web.ViewModels
{
    public class ExpenseExportViewModel
    {
        // Date range for filtering expenses
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        // Amount range for filtering expenses
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        // Category selection for filtering expenses
        public List<int> SelectedCategoriesIds { get; set; } = [];
        public List<SelectListItem> Categories { get; set; } = [];

        // Export format selection (e.g., CSV, Excel)
        public string Format { get; set; } = "xlsx";
    }
}
