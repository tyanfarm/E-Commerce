using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    public class AdminAccountsController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public AdminAccountsController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index() {
            return View(await _context.Accounts.ToListAsync());
        }

        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/AdminAccounts/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Admin/AdminAccounts/Create
        [HttpPost]
        // yêu cầu rằng một mã CSRF token phải được gửi cùng với mỗi yêu cầu POST gửi đến máy chủ
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId, Phone, Email, Password, Salt, Active, FullName, LastLogin, RoleId, CreateDate")] Role role) {
            // ModelState - kiểm tra xem dữ liệu người dùng gửi có hợp lệ không
            if (ModelState.IsValid) {
                _context.Add(role);
                await _context.SaveChangesAsync();

                _notyfService.Success("Create Role Successfully !");

                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // GET: Admin/AdminAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            // FindAsync - tìm kiếm dữ liệu dựa trên PK 
            var role = await _context.Roles.FindAsync(id);

            if (role == null) {
                return NotFound();
            }

            return View(role);
        }

        // POST: Admin/AdminAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId, RoleName, Description")] Role role) {
            if (id != role.RoleId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Update Role Successfully !");
                }
                catch (DbUpdateConcurrencyException) {
                    if (!RoleExists(role.RoleId)) {
                        _notyfService.Success("ERROR !!");

                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                // Update thành công thì chuyển về trang chính
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var role = await _context.Roles.FirstOrDefaultAsync(m => m.RoleId == id);

            if (role == null) {
                return NotFound();
            }

            return View(role);
        }

        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var role = await _context.Roles.FindAsync(id);

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            _notyfService.Success("Delete role successfully !");

            return RedirectToAction(nameof(Index));
        }

        private bool RoleExists(int id) {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}