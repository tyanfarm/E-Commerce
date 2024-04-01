using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace E_Commerce.Controllers {
    public class ProductController : Controller {
        private readonly EcommerceContext _context;

        public ProductController(EcommerceContext context) {
            _context = context;
        }

        public IActionResult Index(int? page) 
        {
            try {
                // Pagination
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 10;
                var listProducts = _context.Products.AsNoTracking()
                                                    .OrderByDescending(p => p.ProductId);

                PagedList<Product> models = new PagedList<Product>(listProducts, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;

                return View(models);
            }
            catch {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("/product-{id}.html", Name="productDetails")] 
        public async Task<IActionResult> Details(int id) {
            try {
                var product = await _context.Products.Include(p => p.Cat)
                                                    .FirstOrDefaultAsync(p => p.ProductId == id);

                if (product == null) {
                    return RedirectToAction("Index");
                }

                return View(product);
            }
            catch {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> List(int catId, int page=1) {
            try {
                // Pagination
                var pageSize = 10;
                var category = await _context.Categories.FindAsync(catId);
                var listProducts = _context.Products.AsNoTracking()
                                                    .Where(p => p.CatId == catId)
                                                    .OrderByDescending(p => p.ProductId);

                PagedList<Product> models = new PagedList<Product>(listProducts, page, pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = category;

                return View(models);
            }
            catch {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}