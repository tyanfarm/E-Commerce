using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // GET: Admin/Authenticate/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl=null) {
            // Kiểm tra xem nếu user đã từng login và phiên đăng nhập vẫn còn thì chuyển qua dashboard
            var customerId = HttpContext.Session.GetString("AdminId");

            if (customerId != null) {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // [HttpPost]
        // [AllowAnonymous]
        // [Route("login.html", Name="UserLogin")]
        // public async Task<IActionResult> Login(LoginViewModel account, string returnUrl=null) {
        //     try {
        //         if (ModelState.IsValid) {
        //             // Validate username
        //             bool isEmail = Utilities.isValidEmail(account.UserName);

        //             if (!isEmail) {
        //                 return View(account);
        //             }

        //             // lấy ra account customer giống với login
        //             var customer = _context.Customers.AsNoTracking()
        //                                                 .SingleOrDefault(c => c.Email.Trim() == account.UserName);
                    
        //             if (customer == null) {
        //                 _notyfService.Warning("You haven't registered yet");
        //                 return RedirectToAction("Register", "UserAccount");
        //             }

        //             // Check pass login
        //             string passLogin = (account.Password + customer.Salt.Trim()).ToMD5();

        //             if (customer.Password != passLogin) {
        //                 _notyfService.Error("Invalid username or password");
                        
        //                 return View(account);
        //             }

        //             // Kiểm tra account có bị disabled không
        //             if (customer.Active == false) {
        //                 _notyfService.Error("Account is disabled");

        //                 return View(account);
        //             }

        //             // Save session customer ID
        //             // Lưu customerID mới register vào session có key="CustomerId"
        //             HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
        //             var userAccountId = HttpContext.Session.GetString("CustomerId");

        //             // Identity - Authenticate & Authorize 
        //             var claims = new List<Claim>
        //             {
        //                 new Claim(ClaimTypes.Name, customer.FullName),
        //                 new Claim("CustomerId", customer.CustomerId.ToString())
        //             };

        //             // Danh tính 1 customer
        //             ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");

        //             // Xác thực danh tính customer
        //             ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //             // Đánh dấu đã xác thực customer
        //             await HttpContext.SignInAsync(claimsPrincipal);
                    
        //             _notyfService.Success("Login successfully");

        //             if (returnUrl == "/checkout.html") {
        //                 return RedirectToAction("Index", "Checkout");
        //             }

        //             return RedirectToAction("Dashboard", "UserAccount");
        //         }
        //     }
        //     catch {
        //         _notyfService.Error("Invalid username or password");

        //         return RedirectToAction("Register", "UserAccount");
        //     }

        //     return View(account);
        // }
    }
}
