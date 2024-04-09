using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Extension;
using E_Commerce.Helper;
using E_Commerce.Models;
using E_Commerce.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

namespace E_Commerce.Controllers {
    [Authorize]
    public class UserAccountController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public UserAccountController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }
        
        public IActionResult Index() 
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string phone) {
            try {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(c => c.Phone.ToLower() == phone.ToLower());

                if (customer != null) {
                    return Json(data: "Phone number: " + phone + " has been used " );
                }

                return Json(data: true);
            }
            catch {
                return Json(data: true);
            }
        } 

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string email) {
            try {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(c => c.Email.ToLower() == email.ToLower());

                if (customer != null) {
                    return Json(data: "Email: " + email + " has been used " );
                }

                return Json(data: true);
            }
            catch {
                return Json(data: true);
            }
        }

        // ------ DASHBOARD ------
        [Route("my-account.html", Name="Dashboard")]
        public IActionResult Dashboard() {
            var customerId = HttpContext.Session.GetString("CustomerId");

            if (customerId != null) {
                var account = _context.Customers.AsNoTracking()
                                                .SingleOrDefault(c => c.CustomerId == Convert.ToInt32(customerId));

                if (account != null) {
                    var listOrders = _context.Orders.AsNoTracking()
                                                    .Include(o => o.TransactStatus)
                                                    .Where(o => o.CustomerId == account.CustomerId)
                                                    .OrderByDescending(o => o.OrderId)
                                                    .ToList();

                    ViewBag.listOrders = listOrders;

                    return View(account);
                }
            }

            return RedirectToAction("Login");
        }

        // ------ REGISTER ------
        [HttpGet]
        [AllowAnonymous]
        [Route("register.html", Name="UserRegister")]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register.html", Name="UserRegister")]
        public async Task<IActionResult> Register(RegisterViewModel info) {
            try {
                if (ModelState.IsValid) {
                    string salt = Utilities.GetRandomKey();

                    Customer customer = new Customer {
                        FullName = info.FullName,
                        Phone = info.Phone.Trim().ToLower(),
                        Email = info.Email.Trim().ToLower(),
                        // Hash password
                        Password = (info.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now
                    };

                    try {
                        // add customer Account to DB
                        _context.Add(customer);
                        await _context.SaveChangesAsync();

                        // Save session customer ID
                        // Lưu customerID mới register vào session có key="CustomerId"
                        HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                        var userAccountId = HttpContext.Session.GetString("CustomerId");

                        // Identity - Authenticate & Authorize 
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, customer.FullName),
                            new Claim("CustomerId", customer.CustomerId.ToString())
                        };

                        // Danh tính 1 customer
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");

                        // Xác thực danh tính customer
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        // Tạo phiên làm việc mới cho user
                        await HttpContext.SignInAsync(claimsPrincipal);
                        
                        _notyfService.Success("Login successfully");

                        return RedirectToAction("Dashboard", "UserAccount");
                    }
                    // Nếu không AddAuthentication thì sẽ bị lỗi ở đây - không add vào DB được
                    catch {
                        _notyfService.Error("Invalid information !");

                        return RedirectToAction("Register", "UserAccount");
                    }
                }
                else {
                    _notyfService.Error("Incomplete information");

                    return View(info);
                }
            }
            catch {

                return View(info);
            }
        }

        // ------ LOGIN ------
        [HttpGet]
        [AllowAnonymous]
        [Route("login.html", Name="UserLogin")]
        public IActionResult Login(string returnUrl=null) {
            // Kiểm tra xem nếu user đã từng login và phiên đăng nhập vẫn còn thì chuyển qua dashboard
            var customerId = HttpContext.Session.GetString("CustomerId");

            if (customerId != null) {
                return RedirectToAction("Dashboard", "UserAccount");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login.html", Name="UserLogin")]
        public async Task<IActionResult> Login(LoginViewModel account, string returnUrl=null) {
            try {
                if (ModelState.IsValid) {
                    // Validate username
                    bool isEmail = Utilities.isValidEmail(account.UserName);

                    if (!isEmail) {
                        return View(account);
                    }

                    // lấy ra account customer giống với login
                    var customer = _context.Customers.AsNoTracking()
                                                        .SingleOrDefault(c => c.Email.Trim() == account.UserName);
                    
                    if (customer == null) {
                        _notyfService.Warning("You haven't registered yet");
                        return RedirectToAction("Register", "UserAccount");
                    }

                    // Check pass login
                    string passLogin = (account.Password + customer.Salt.Trim()).ToMD5();

                    if (customer.Password != passLogin) {
                        _notyfService.Error("Invalid username or password");
                        
                        return View(account);
                    }

                    // Kiểm tra account có bị disabled không
                    if (customer.Active == false) {
                        _notyfService.Error("Account is disabled");

                        return View(account);
                    }

                    // Save session customer ID
                    // Lưu customerID mới register vào session có key="CustomerId"
                    HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                    var userAccountId = HttpContext.Session.GetString("CustomerId");

                    // Identity - Authenticate & Authorize 
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, customer.FullName),
                        new Claim("CustomerId", customer.CustomerId.ToString())
                    };

                    // Danh tính 1 customer
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");

                    // Xác thực danh tính customer
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Đánh dấu đã xác thực customer
                    await HttpContext.SignInAsync(claimsPrincipal);
                    
                    _notyfService.Success("Login successfully");

                    if (returnUrl == "/checkout.html") {
                        return RedirectToAction("Index", "Checkout");
                    }

                    return RedirectToAction("Dashboard", "UserAccount");
                }
            }
            catch {
                _notyfService.Error("Invalid username or password");

                return RedirectToAction("Register", "UserAccount");
            }

            return View(account);
        }

        // ------ LOGOUT ------
        [HttpGet]
        [Route("logout.html", Name="logout")]
        public IActionResult Logout() {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }

        // ------ CHANGE PASSWORD ------
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            try {
                // Lấy account đang login của user
                var accountId = int.Parse(HttpContext.Session.GetString("CustomerId"));

                if (accountId == null) {
                    return RedirectToAction("Login", "UserAccount");
                }

                var account = _context.Customers.Find(accountId);

                if (account == null) {
                    return RedirectToAction("Index", "Home");
                }

                var password = (model.PasswordNow.Trim() + account.Salt.Trim()).ToMD5();

                // Password Now đúng thì change password
                if (password == account.Password) {
                    string newPassword = (model.Password.Trim() + account.Salt.Trim()).ToMD5();

                    account.Password = newPassword;
                    
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    
                    _notyfService.Success("Update password successfully");

                    return RedirectToAction("Dashboard", "UserAccount");
                }
            }
            catch {
                _notyfService.Error("Change password failed");
                
                return RedirectToAction("Dashboard", "UserAccount");
            }
            _notyfService.Error("Change password failed");

            return RedirectToAction("Dashboard", "UserAccount");
        }

    }
}