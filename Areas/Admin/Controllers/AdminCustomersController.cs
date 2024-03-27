using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

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
            // var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            // var pageSize = 20;
            // var listCustomers = _context.Customers.AsNoTracking()
            //                                         .Include(x => x.Location)
            //                                         .OrderByDescending(x => x.CreateDate);

            // PagedList<Customer> models = new PagedList<Customer>(listCustomers, pageNumber, pageSize);
            // ViewBag.CurrentPage = pageNumber;

            // return View(models);

            // var locations = await _context.Locations.ToListAsync();
            // var customers = await _context.Customers.ToListAsync();
            
            // foreach (var a in customers) {
            //     a.Location = locations.FirstOrDefault(r => r.LocationId == a.LocationId);
            // }
            // return View(customers);
            var ecommerceContext = _context.Customers.Include(c => c.Location);
            return View(await ecommerceContext.ToListAsync());
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