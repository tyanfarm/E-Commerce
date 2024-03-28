using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    public class AdminPagesController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public AdminPagesController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminPages
        public async Task<IActionResult> Index(int? page) {
            // Pagination
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var listPages = _context.Pages.AsNoTracking()
                                            .OrderByDescending(x => x.PageId);

            PagedList<Page> models = new PagedList<Page>(listPages, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }

        // GET: Admin/AdminPages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FirstOrDefaultAsync(x => x.PageId == id);

            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }
    }
}