using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Areas.Admin.DTO;
using E_Commerce.Extension;
using E_Commerce.Helper;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize]
    public class AuthenticateController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public AuthenticateController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: /admin-register.html
        [HttpGet]
        [AllowAnonymous]
        [Route("admin-register.html", Name = "Admin Register")]
        public IActionResult Register() {
            return View();
        }

        // POST: /admin-register.html
        [HttpPost]
        [AllowAnonymous]
        [Route("admin-register.html", Name = "Admin Register")]
        public async Task<IActionResult> Register(AccountDTO registerAcccount) {
            try {
                if (ModelState.IsValid) {
                    string salt  = Utilities.GetRandomKey();

                    Account account = new Account {
                        Username = registerAcccount.Username.Trim().ToLower(),
                        // Hash password
                        Password = (registerAcccount.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        RoleId = registerAcccount.RoleId,
                        CreateDate = DateTime.Now
                    };

                    // Find role of account registered
                    var role = _context.Roles.AsNoTracking()
                                                    .SingleOrDefault(r => r.RoleId == registerAcccount.RoleId);

                    if (string.IsNullOrEmpty(role.RoleName)) {
                        _notyfService.Error("INVALID Role !");

                        return RedirectToAction("Register", "Authenticate");
                    }

                    try {
                        // add customer Account to DB
                        _context.Add(account);
                        await _context.SaveChangesAsync();

                        // Save session customer ID
                        // Lưu customerID mới register vào session có key="CustomerId"
                        HttpContext.Session.SetString("AdminId", account.AccountId.ToString());
                        var adminAccountId = HttpContext.Session.GetString("AdminId");

                        // Identity - Authenticate & Authorize 
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, registerAcccount.Username),
                            new Claim("AdminId", adminAccountId),
                            new Claim(ClaimTypes.Role, role.RoleName)
                        };

                        // Danh tính 1 customer
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "adminLogin");

                        // Xác thực danh tính customer
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Tạo phiên làm việc mới cho user
                        // --SignInAsync(AuthenticationScheme, claimsPrincipal)--
                        await HttpContext.SignInAsync(claimsPrincipal);
                        
                        _notyfService.Success("Register successfully");

                        return RedirectToAction("Index", "Home");
                    }
                    // Nếu không AddAuthentication thì sẽ bị lỗi ở đây - không add vào DB được
                    catch {
                        _notyfService.Error("Register Fail !");

                        return RedirectToAction("Register", "Authenticate");
                    }
                }
                else {
                    _notyfService.Error("Incomplete information");

                    return View(registerAcccount);
                }
            }
            catch {

                return View(registerAcccount);
            }
        }

        // GET: /admin-login.html
        [HttpGet]
        [AllowAnonymous]
        [Route("admin-login.html", Name="Admin Login")]
        public IActionResult Login(string returnUrl=null) {
            // Kiểm tra xem nếu Admin đã từng login và phiên đăng nhập vẫn còn thì chuyển qua dashboard
            var accountId = HttpContext.Session.GetString("AdminId");

            if (accountId != null) {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: /admin-login.html        
        [HttpPost]
        [AllowAnonymous]
        [Route("admin-login.html", Name="Admin Login")]
        public async Task<IActionResult> Login(AccountDTO loginAccount, string returnUrl=null) {
            try {
                // Vì ModelState sẽ kiểm tra loginAccount có được điền hết các giá trị attribute 
                // Nhưng ta chỉ truyền Username và Password
                if (ModelState.IsValid || (loginAccount.Username != null && loginAccount.Password != null)) {
                    // lấy ra account customer giống với login
                    var admin = _context.Accounts.AsNoTracking()
                                                    .SingleOrDefault(a => a.Username.Trim() == loginAccount.Username);
                    
                    if (admin == null) {
                        _notyfService.Warning("INVALID account");
                        return RedirectToAction("Index", "Home");
                    }

                    // Check pass login
                    string passLogin = (loginAccount.Password + admin.Salt.Trim()).ToMD5();

                    if (admin.Password != passLogin) {
                        _notyfService.Error("Invalid username or password");
                        
                        return View(loginAccount);
                    }

                    // Kiểm tra account có bị disabled không
                    if (admin.Active == false) {
                        _notyfService.Error("Account is disabled");

                        return View(loginAccount);
                    }

                    // Ánh xạ với account admin trong DB
                    var role = _context.Roles.AsNoTracking()
                                                .SingleOrDefault(r => r.RoleId == admin.RoleId);

                    if (string.IsNullOrEmpty(role.RoleName)) {
                        _notyfService.Error("INVALID Role !");

                        return RedirectToAction("Login", "Authenticate");
                    }

                    // Save session customer ID
                    // Lưu customerID mới register vào session có key="CustomerId"
                    HttpContext.Session.SetString("AdminId", admin.AccountId.ToString());
                    var adminAccountId = HttpContext.Session.GetString("AdminId");

                    // Identity - Authenticate & Authorize 
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginAccount.Username),
                            new Claim("AdminId", adminAccountId),
                            new Claim(ClaimTypes.Role, role.RoleName)
                    };

                    // Danh tính 1 customer
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "adminLogin");

                    // Xác thực danh tính customer
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Đánh dấu đã xác thực customer
                    await HttpContext.SignInAsync(claimsPrincipal);
                    
                    _notyfService.Success("Login successfully");

                    return RedirectToAction("Index", "Home");
                }
            }
            catch {
                _notyfService.Error("Invalid username or password");

                return RedirectToAction("Login", "Authenticate");
            }

            _notyfService.Error("ModelState ERROR");

            return View(loginAccount);
        }

        [HttpGet]
        [Route("admin-logout.html", Name = "Sign Out")]
        public IActionResult Logout() {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("Login", "Authenticate");
        }
    }
}
