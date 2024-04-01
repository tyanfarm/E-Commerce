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

                // AsNoTracking() - không cần _context theo dõi
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

                var listProducts = _context.Products.AsNoTracking()
                                                    .Where(p => p.CatId == product.CatId && p.ProductId != id && p.Active == true)
                                                    .OrderByDescending(p => p.ProductId)
                                                    .Take(4)
                                                    .ToList();

                ViewBag.listProducts = listProducts;

                return View(product);
            }
            catch {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("/category-{catId}.html", Name="listProducts")]
        public async Task<IActionResult> List(int page=1, int catId=0) {
            try {
                List<Product> listProducts = new List<Product>();

                // Pagination
                var pageSize = 10;
                var category = await _context.Categories.FindAsync(catId);

                if (catId != 0) {
                    listProducts = _context.Products.AsNoTracking()
                                                    .Where(p => p.CatId == catId)
                                                    .OrderByDescending(p => p.ProductId)
                                                    .ToList();
                }

                PagedList<Product> models = new PagedList<Product>(listProducts.AsQueryable(), page, pageSize);

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