using AutoMapper;
using Web.DTOs.Category;
using Web.DTOs.Expense;
using Web.Models;

namespace Web.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Expense Mappings
            CreateMap<Expense, ExpenseDTO>();
            CreateMap<ExpenseDTO, Expense>();
            CreateMap<ExpenseCreateDTO, Expense>();

            // Category Mappings
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<CategoryCreateDTO, Category>();
        }

    }
}
