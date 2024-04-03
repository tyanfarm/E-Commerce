using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Extension;
using E_Commerce.Helper;
using E_Commerce.Models;
using E_Commerce.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

namespace E_Commerce.Controllers {
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
                        Phone = info.Phone,
                        Email = info.Email,
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

                        // Đánh dấu đã xác thực customer
                        await HttpContext.SignInAsync(claimsPrincipal);

                        return RedirectToAction("Dashboard", "UserAccount");
                    }
                    catch {
                        return RedirectToAction("Register", "UserAccount");
                    }
                }
                else {
                    return View(info);
                }
            }
            catch {
                return View(info);
            }
        }
    }
}