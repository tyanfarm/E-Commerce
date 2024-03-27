using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    public class AdminCustomersController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public AdminCustomersController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminCustomers
        public async Task<IActionResult> Index() {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: Admin/AdminCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }
    }
}