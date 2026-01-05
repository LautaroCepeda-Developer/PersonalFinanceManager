using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; } = DateTime.Today;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }
    }
}
