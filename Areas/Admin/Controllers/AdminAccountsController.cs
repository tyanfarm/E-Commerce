using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminAccountsController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public AdminAccountsController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index() {
            // var roles = await _context.Roles.ToListAsync();
            // var accounts = await _context.Accounts.ToListAsync();
            
            // foreach (var a in accounts) {
            //     a.Role = roles.FirstOrDefault(r => r.RoleId == a.RoleId);
            // }
            // return View(accounts);

            // SelectList - Tạo các đối tượng cho thẻ <select>
            ViewData["AccessPermission"] = new SelectList(_context.Roles, "RoleId", "Description");

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem() {Text="Active", Value="1"});
            status.Add(new SelectListItem() {Text="Block", Value="0"});

            ViewData["Status"] = status;

            var ecommerceContext = _context.Accounts.Include(a => a.Role);
            return View(await ecommerceContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("AccountId, Phone, Email, Password, Salt, Active, FullName, LastLogin, RoleId, CreateDate, Role")] Account account) {
            // ModelState - kiểm tra xem dữ liệu người dùng gửi có hợp lệ không
            if (ModelState.IsValid) {
                _context.Add(account);
                await _context.SaveChangesAsync();

                _notyfService.Success("Create Role Successfully !");

                return RedirectToAction(nameof(Index));
            }

            return View(account);
        }

        // GET: Admin/AdminAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            // FindAsync - tìm kiếm dữ liệu dựa trên PK 
            var account = await _context.Accounts.FindAsync(id);

            if (account == null) {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/AdminAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId, Phone, Email, Password, Salt, Active, FullName, LastLogin, RoleId, CreateDate, Role")] Account account) {
            if (id != account.AccountId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Update Account Successfully !");
                }
                catch (DbUpdateConcurrencyException) {
                    if (!AccountExists(account.AccountId)) {
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

            return View(account);
        }

        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(m => m.AccountId == id);

            if (account == null) {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var account = await _context.Accounts.FindAsync(id);

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            _notyfService.Success("Delete role successfully !");

            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id) {
            return _context.Accounts.Any(a => a.AccountId == id);
        }
    }
}