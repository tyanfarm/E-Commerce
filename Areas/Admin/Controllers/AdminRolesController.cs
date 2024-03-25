using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    public class AdminRolesController : Controller {
        private readonly EcommerceContext _context;

        public AdminRolesController(EcommerceContext context) {
            _context = context;
        }

        // GET: Admin/AdminRoles
        public async Task<IActionResult> Index() {
            return View(await _context.Roles.ToListAsync());
        }

        // GET: Admin/AdminRoles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }
    }
}