using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SearchController : Controller {
        private readonly EcommerceContext _context;

        public SearchController(EcommerceContext context) {
            _context = context;
        }

        // POST: Admin/Search/FindProduct
        [HttpPost]
        public IActionResult FindProduct(string keyword) {
            List<Product> listProducts = new List<Product>();

            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1) {
                return PartialView("ListProductsSearchPartial", null);
            }

            // Search product
            listProducts = _context.Products.AsNoTracking()
                                            .Include(p => p.Cat)
                                            .Where(p => p.ProductName.Contains(keyword))
                                            .OrderByDescending(p => p.ProductName)
                                            .ToList();

            if (listProducts == null) {
                return PartialView("ListProductsSearchPartial", null);
            }
            else {
                return PartialView("ListProductsSearchPartial", listProducts);
            }
        }
    }
}