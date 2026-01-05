using Microsoft.AspNetCore.Identity;

namespace Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
