using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.DTOs.Expense;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    [Authorize]
    public class ExpenseController(IExpenseService service, ICategoryService categoryService, UserManager<IdentityUser> userManager) : Controller
    {
        private readonly IExpenseService _service = service;
        private readonly ICategoryService _categoryService = categoryService;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public async Task<IActionResult> Index(DateTime? from, DateTime? to, int? categoryId)
        {
            // User manager automatically throws if user is not found
            var user = await _userManager.GetUserAsync(User);

            // User can be null but userManager.GetUserAsync will throw before that happens (! to supress nullable warning)
            var expenses = await _service.GetUserExpensesAsync(user!.Id, from, to, categoryId);

            var categories = await _categoryService.GetUserCategoriesAsync(user!.Id);

            ExpensesIndexViewModel viewModel = new()
            {
                Expenses = expenses,
                From = from,
                To = to,
                CategoryId = categoryId,
                TotalAmount = expenses.Sum(e => e.Amount),
                Categories = categories
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var categories = await _categoryService.GetUserCategoriesAsync(user!.Id);

            var model = new ExpenseCreateViewModel
            {
                Expense = new ExpenseCreateDTO { Date = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc) },
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseCreateViewModel viewModel)
        {
            var dto = viewModel.Expense;
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var categories = await _categoryService.GetUserCategoriesAsync(user!.Id);
                viewModel.Categories = [.. categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })];
                viewModel.Expense.Date = dto.Date == default ? DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc) : dto.Date;

                return View(viewModel);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var expense = new ExpenseCreateDTO
            {
                Description = dto.Description,
                Amount = dto.Amount,
                Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                CategoryId = dto.CategoryId,
                UserId = currentUser!.Id
            };

            await _service.AddExpenseAsync(expense);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _service.GetExpenseByIdAsync(id);
            if (expense == null) return NotFound();
            var expenseUpdateDTO = new ExpenseUpdateDTO
            {
                Id = expense.Id,
                Amount = expense.Amount,
                CategoryId = expense.CategoryId,
                Date = expense.Date,
                Description = expense.Description,
                UserId = expense.UserId
            };
            var categories = await _categoryService.GetUserCategoriesAsync(expense.UserId!);

            var vm = new ExpenseUpdateViewModel
            {
                Expense = expenseUpdateDTO,
                Categories = [.. categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })]
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExpenseUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            await _service.UpdateExpenseAsync(viewModel.Expense);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _service.GetExpenseByIdAsync(id);
            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var result = await _service.DeleteExpenseAsync(id);

            if (!result.Succeeded)
                {
                ModelState.AddModelError("Id",result.Message!);
                var expense = await _service.GetExpenseByIdAsync(id);
                return View("Delete", expense);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
