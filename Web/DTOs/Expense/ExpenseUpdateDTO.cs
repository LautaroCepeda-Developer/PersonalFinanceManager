namespace Web.DTOs.Expense
{
    public class ExpenseUpdateDTO
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public int CategoryId { get; set; }
    }
}
