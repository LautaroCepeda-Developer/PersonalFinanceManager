using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Services;

namespace Web.Controllers
{
    [Authorize]
    public class DashboardController(UserManager<IdentityUser> userManager, IExpenseService expenseService) : Controller
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IExpenseService _expenseService = expenseService;

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var expenses = await _expenseService.GetUserExpensesAsync(user!.Id);
            var grouped = expenses
                .GroupBy(e => e.Category!.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(e => e.Amount)
                })
                .ToList();

            return View(grouped);
        }


    }
}
