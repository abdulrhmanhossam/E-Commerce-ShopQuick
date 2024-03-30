using DataBaseAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelClasses;

namespace E_CommerceWebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        // To Go To View
        [Authorize]
        public IActionResult Upsert(int? id)
        {
            if (id == 0)
            {
                Category category = new Category();
                return View(category);
            }
            else
            {
                var categories = _context.Categories.FirstOrDefault(c => c.Id == id);
                return View(categories);
            }
        }

        // To Check if This Category Found Or Create New Category
        [HttpPost]
        public async Task<IActionResult> Upsert(int? id, Category category)
        {
            if (id == null)
            {
                var foundCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName);

                if (foundCategory != null)
                {
                    TempData["alertMassage"] = $"{category.CategoryName} is a existing item found in the list."
                    + " so not added to the list";
                    return RedirectToAction("Index");
                }
                await _context.Categories.AddAsync(category);

                TempData["alertMassage"] = $"{category.CategoryName} has add into the category list";
            }
            else
            {
                var updatedCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == id);
                updatedCategory!.CategoryName = category.CategoryName;

                TempData["alertMassage"] = $"{updatedCategory.CategoryName} has updated into the category list";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories
                .FirstOrDefault(c => c.Id == id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Category category)
        {
            var deletedCategory = _context.Categories
                .FirstOrDefault(c => c.Id == category.Id);
            _context.Categories.Remove(deletedCategory!);

            TempData["alertMassage"] = $"{deletedCategory!.CategoryName} has deleted form the category list";

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
