using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Helper;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminProductsController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public AdminProductsController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminProducts
        public async Task<IActionResult> Index(int page=1, int CatId=0) {
            // Pagination
            var pageNumber = page;
            var pageSize = 10;

            List<Product> listProducts = new List<Product>();

            // Filter by Category ID
            if (CatId != 0) {
                listProducts = _context.Products.AsNoTracking()
                                                .Where(p => p.CatId == CatId)
                                                .Include(p => p.Cat)
                                                .OrderByDescending(p => p.ProductId).ToList();
            }
            else {
                listProducts = _context.Products.AsNoTracking()
                                                .Include(p => p.Cat)
                                                .OrderByDescending(p => p.ProductId).ToList();
            }

            // PagedList không nhận data type List
            PagedList<Product> models = new PagedList<Product>(listProducts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentCatId = CatId;

            // public SelectList(IEnumerable items, string dataValueField, string dataTextField, object selectedValue);
            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName", CatId);

            return View(models);
        }

        // Use to create urls for FE reload page when choose category
        public IActionResult Filter(int CatId = 0) {
            var url = $"/Admin/AdminProducts?CatId={CatId}";

            if (CatId == 0) {
                url = $"/Admin/AdminProducts";
            }

            return Json(new {status = "success", redirectUrl = url});
        }

        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Cat).FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create() 
        {
            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName");

            return View();
        }

        // POST: Admin/AdminProducts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitsInStock")] Product product, IFormFile fThumb)
        {
            if (ModelState.IsValid || fThumb == null) {
                // format product name
                product.ProductName = Utilities.ToTitleCase(product.ProductName);

                if (fThumb != null) {
                    // Lấy phần mở rộng (a.jpg thì lấy `.jpg`)
                    string extension = Path.GetExtension(fThumb.FileName);
                    // format img name which doesn't have strange syntax
                    string image = Utilities.SEOUrl(product.ProductName) + extension;

                    product.Thumb = await Utilities.UploadFile(fThumb, "products", image.ToLower());
                    product.Thumb = image;
                }

                if (string.IsNullOrEmpty(product.Thumb)) {
                    product.Thumb = "default.png";
                }

                product.Alias = Utilities.SEOUrl(product.ProductName);
                product.DateCreated = DateTime.Now; 

                _context.Add(product);
                await _context.SaveChangesAsync();

                _notyfService.Success("Create Successfully !");

                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("ERROR !");

            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);

            return View(product);
        }

        // GET: Admin/AdminProducts/Edit
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

            if (product == null) {
                return NotFound();
            }

            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);

            return View(product);
        }

        // POST: Admin/AdminProducts/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitsInStock")] Product product, IFormFile fThumb) {
            if (id != product.ProductId) {
                return NotFound();
            }

            // Admin doesn't want to update thumb still valid
            if (ModelState.IsValid || fThumb == null) 
            {
                try {
                    // format product name
                    product.ProductName = Utilities.ToTitleCase(product.ProductName);

                    if (fThumb != null) {
                        // Lấy phần mở rộng (a.jpg thì lấy `.jpg`)
                        string extension = Path.GetExtension(fThumb.FileName);
                        // format img name which doesn't have strange syntax
                        string image = Utilities.SEOUrl(product.ProductName) + extension;

                        product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    }

                    if (string.IsNullOrEmpty(product.Thumb)) {
                        product.Thumb = "default.png";
                    }

                    product.Alias = Utilities.SEOUrl(product.ProductName);
                    product.DateCreated = DateTime.Now; 

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    _notyfService.Success("Update Successfully !");
                }
                catch (DbUpdateConcurrencyException) {
                    if (!ProductExists(product.ProductId)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("ERROR !");

            ViewData["Category"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);

            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var product = await _context.Products.FindAsync(id);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _notyfService.Success("Delete successfully !");

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id) {
            return _context.Products.Any(p => p.ProductId == id);
        }
    } 
}