using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Web.DTOs.Category;
using Web.DTOs;
using Web.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Authorize]
    public class CategoryController(ICategoryService service, UserManager<IdentityUser> userManager) : Controller
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly ICategoryService _service = service;

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var categories = await _service.GetUserCategoriesAsync(user!.Id);
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            dto.UserId = user!.Id;

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _service.AddCategoryAsync(dto);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(dto.Name), result.Message!);
                return View(dto);
            }

            if (result.IsRestored)
            {
                TempData["LayoutAlert"] = JsonSerializer.Serialize(new TempDataAlert { Message = result.Message!, Type = TempDataAlert.AlertType.Sucess });
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _service.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDTO dto)
        {

            var user = await _userManager.GetUserAsync(User);
            dto.UserId = user!.Id;

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _service.UpdateCategoryAsync(dto);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(dto.Name), result.Message!);
                return View(dto);
            }

            if (result.IsRestored)
            {
                TempData["LayoutAlert"] = JsonSerializer.Serialize(new TempDataAlert { Message = result.Message!, Type = TempDataAlert.AlertType.Info });
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _service.GetCategoryByIdAsync(id);
            if (category is null) return NotFound();

            return View(category);
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            await _service.DeleteCategoryByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Restore()
        {
            var user = await _userManager.GetUserAsync(User);
            var categories = await _service.GetUserSoftDeletedCategoriesAsync(user!.Id);
            return View(categories);
        }

        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCategory(int id)
        {
            var result = await _service.RestoreCategoryAsync(id);

            TempData["LayoutAlert"] = JsonSerializer.Serialize(new TempDataAlert { Message = result.Message!, Type = TempDataAlert.AlertType.Sucess });
            return RedirectToAction(nameof(Index));
        }
    }
}
