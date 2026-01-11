using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Services;
using Web.ViewModels;

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

            var expensesByCategory = await _expenseService.GetCurrentMonthExpensesByCategoryAsync(user!.Id);

            var dailyExpenses = await _expenseService.GetLast30DaysExpensesAsync(user.Id);

            var monthlyExpenses = await _expenseService.GetLast12MonthsExpensesAsync(user.Id);

            var viewModel = new DashboardViewModel
            {
                ExpensesByCategory = expensesByCategory,
                DailyExpenses = dailyExpenses,
                MonthlyExpenses = monthlyExpenses
            };
            return View(viewModel);
        }
    }
}
