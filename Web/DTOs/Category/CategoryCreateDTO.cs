using System.ComponentModel.DataAnnotations;

namespace Web.DTOs.Category
{
    public class CategoryCreateDTO
    {
        [Required(ErrorMessage = "NameRequiredError")]
        public string Name { get; set; } = string.Empty;
        public string? UserId { get; set; }
    }
}
