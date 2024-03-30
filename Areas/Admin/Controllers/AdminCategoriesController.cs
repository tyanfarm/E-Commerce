using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Helper;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    public class AdminCategoriesController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public AdminCategoriesController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminCategories
        public async Task<IActionResult> Index(int? page) {
            // Pagination
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var listCategories = _context.Categories.AsNoTracking()
                                                    .OrderByDescending(c => c.CatId);

            PagedList<Category> models = new PagedList<Category>(listCategories, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }

        // GET: Admin/AdminCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatId == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/AdminCategories/Create
        public IActionResult Create() 
        {
            return View();
        }

        // POST: Admin/AdminCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Description,ParentId,Levels,Ordering,Published,Thumb,Title,Alias,MetaDesc,MetaKey,Cover,SchemaMarkup")] Category category, IFormFile fThumb)
        {
            if (ModelState.IsValid) {
                // format product name
                category.CatName = Utilities.ToTitleCase(category.CatName);

                if (fThumb != null) {
                    // Lấy phần mở rộng (a.jpg thì lấy `.jpg`)
                    string extension = Path.GetExtension(fThumb.FileName);
                    // format img name which doesn't have strange syntax
                    string image = Utilities.SEOUrl(category.CatName) + extension;

                    category.Thumb = await Utilities.UploadFile(fThumb, "categories", image.ToLower());
                    category.Thumb = image;
                }

                if (string.IsNullOrEmpty(category.Thumb)) {
                    category.Thumb = "default.png";
                }

                category.Alias = Utilities.SEOUrl(category.CatName);

                _context.Add(category);
                await _context.SaveChangesAsync();

                _notyfService.Success("Create Successfully !");

                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("ERROR !");

            return View(category);
        }

        // GET: Admin/AdminCategories/Edit
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var category = _context.Categories.FirstOrDefault(c => c.CatId == id);

            if (category == null) {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("CatId,CatName,Description,ParentId,Levels,Ordering,Published,Thumb,Title,Alias,MetaDesc,MetaKey,Cover,SchemaMarkup")] Category category, IFormFile fThumb) {
            if (id != category.CatId) {
                return NotFound();
            }

            if (ModelState.IsValid || fThumb == null) 
            {
                try {
                    // format product name
                    category.CatName = Utilities.ToTitleCase(category.CatName);

                    if (fThumb != null) {
                        // Lấy phần mở rộng (a.jpg thì lấy `.jpg`)
                        string extension = Path.GetExtension(fThumb.FileName);
                        // format img name which doesn't have strange syntax
                        string image = Utilities.SEOUrl(category.CatName) + extension;

                        category.Thumb = await Utilities.UploadFile(fThumb, "categories", image.ToLower());
                    }

                    if (string.IsNullOrEmpty(category.Thumb)) {
                        category.Thumb = "default.png";
                    }

                    category.Alias = Utilities.SEOUrl(category.CatName);

                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    _notyfService.Success("Update Successfully !");
                }
                catch (DbUpdateConcurrencyException) {
                    if (!CategoryExists(category.CatId)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("ERROR !");

            return View(category);
        }

        // GET: Admin/AdminCategories/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatId == id);

            if (category == null) {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var category = await _context.Categories.FindAsync(id);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _notyfService.Success("Delete successfully !");

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id) {
            return _context.Categories.Any(c => c.CatId == id);
        }
    } 
}