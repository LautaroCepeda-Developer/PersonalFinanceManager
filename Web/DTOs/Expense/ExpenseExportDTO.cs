namespace Web.DTOs.Expense
{
    public class ExpenseExportDTO
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public List<int> CategoryIds { get; set; } = [];

        public string UserId { get; set; } = string.Empty;

        public string Format { get; set; } = "xlsx";
    }
}
