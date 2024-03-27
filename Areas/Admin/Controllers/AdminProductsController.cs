using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    public class AdminProductsController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public AdminProductsController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminProducts
        public async Task<IActionResult> Index(int? page) {
            // Pagination
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var listProducts = _context.Products.AsNoTracking()
                                                    .Include(p => p.Cat)
                                                    .OrderByDescending(p => p.ProductId);

            PagedList<Product> models = new PagedList<Product>(listProducts, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }

        // GET: Admin/AdminRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FirstOrDefaultAsync(m => m.RoleId == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }
    }
}