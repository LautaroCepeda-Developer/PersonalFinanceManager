using System.ComponentModel.DataAnnotations;
namespace Web.DTOs.Expense
{
    public class ExpenseCreateDTO ()
    {
        public string UserId { get; set; } = string.Empty;


        [Required(ErrorMessage = "DescriptionRequiredError")]
        public string Description { get; set; } = "";


        [Required(ErrorMessage ="AmountRequiredError")]
        [Range(0.01, double.MaxValue, ErrorMessage = "AmountRangeError")]
        public decimal Amount { get; set; }


        [Required(ErrorMessage ="DateRequiredError")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


        [Required(ErrorMessage ="CategoryRequiredError")]
        public int CategoryId { get; set; }
    }
}
