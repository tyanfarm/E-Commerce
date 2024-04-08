using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers {
    public class OrderController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public OrderController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        [Route("/order-details-{id}.html", Name="Order Details")]
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                _notyfService.Error("ERROR");
                
                return RedirectToAction("Index", "Home");
            }

            try {
                var accountId = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(accountId)) {
                    return RedirectToAction("Login", "UserAccount");
                }

                // Kiểm tra khách hàng sở hữu đơn hàng
                var customer = _context.Customers.AsNoTracking()
                                                .SingleOrDefault(c => c.CustomerId == Convert.ToInt32(accountId));
                if (customer == null) {
                    _notyfService.Error("ERROR");
                
                    return RedirectToAction("Index", "Home");
                }

                // Tìm đơn hàng
                var order = _context.Orders.FirstOrDefault(o => o.OrderId == id && o.CustomerId == Convert.ToInt32(accountId));
                if (order == null) {
                    _notyfService.Error("ERROR");
                
                    return RedirectToAction("Index", "Home");
                }

                // Lấy chi tiết đơn hàng
                var orderDetails = _context.Orderdetails.Include(od => od.Product)
                                                        .AsNoTracking()
                                                        .Where(od => od.OrderId == id)
                                                        .OrderBy(od => od.OrderDetailId)
                                                        .ToList();

                return View(orderDetails);
            }
            catch {
                _notyfService.Error("ERROR");
                
                return RedirectToAction("Index", "Home");
            }
        }
    }
}