using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Extension;
using E_Commerce.Models;
using E_Commerce.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers {
    public class CheckoutController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public CheckoutController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        //Khởi tạo cart của user
        public List<CartItemViewModel> shoppingCart {
            //getter
            get {
                var cart = HttpContext.Session.Get<List<CartItemViewModel>>("shoppingCart");

                //Xử lý trường hợp cart == null
                if (cart == default(List<CartItemViewModel>)) {
                    cart = new List<CartItemViewModel>();
                }

                return cart;
            }
        }

        //GET: Checkout/Index
        [Route("checkout.html", Name="Checkout")]
        public IActionResult Index(string returnUrl=null) {
            var cart = HttpContext.Session.Get<List<CartItemViewModel>>("shoppingCart");
            var accountId = HttpContext.Session.GetString("CustomerId");
            ConfirmOrderViewModel model = new ConfirmOrderViewModel();

            // Chưa login
            if (accountId == null) {
                return RedirectToAction("Login", "UserAccount");
            }

            if (accountId != null) {
                var customer = _context.Customers.AsNoTracking()
                                                .SingleOrDefault(c => c.CustomerId == Convert.ToInt32(accountId));
                
                model.CustomerId = customer.CustomerId;
                model.FullName = customer.FullName;
                model.Phone = customer.Phone;
                model.Address = customer.Address;
            }

            ViewBag.shoppingCart = cart;

            return View(model);
        }

        //POST: Checkout/Index
        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public async Task<IActionResult> Index(ConfirmOrderViewModel confirmOrder) {
            try {
                // Get cart
                var cart = HttpContext.Session.Get<List<CartItemViewModel>>("shoppingCart");
                var accountId = HttpContext.Session.GetString("CustomerId");

                // Cập nhật thông tin khách hàng
                // Vì là confirm order nên có thể thông tin đặt hàng của user thay đổi 
                // => Cần update trước khi xác nhận đơn hàng
                if (accountId != null) {
                    var customer = _context.Customers.AsNoTracking()
                                                    .SingleOrDefault(c => c.CustomerId == Convert.ToInt32(accountId));

                    // Update customer
                    customer.FullName = confirmOrder.FullName;
                    customer.Phone = confirmOrder.Phone;
                    customer.Address = confirmOrder.Address;     

                    _context.Update(customer);
                    await _context.SaveChangesAsync();       
                }

                // Lưu đơn hàng
                try {
                    if (ModelState.IsValid) {
                        // Khởi tạo order
                        Order order = new Order();

                        order.CustomerId = confirmOrder.CustomerId;
                        order.Address = confirmOrder.Address;
                        
                        order.OrderDate = DateTime.Now;
                        order.ShipDate = DateTime.Now;
                        order.TransactStatusId = 1;     // New order
                        order.Deleted = false;
                        order.Paid = false;
                        order.TotalMoney = cart.Sum(c => c.totalMoney);

                        _context.Add(order);
                        await _context.SaveChangesAsync();

                        // Lưu thông tin từng đơn sản phẩm trong đơn hàng
                        foreach (var item in cart) {
                            Orderdetail orderDetail = new Orderdetail();

                            orderDetail.OrderId = order.OrderId;
                            orderDetail.ProductId = item.product.ProductId;
                            orderDetail.Quantity = item.quantity;
                            orderDetail.Total = Convert.ToInt32(item.totalMoney);
                            orderDetail.ShipDate = DateTime.Today;

                            _context.Add(orderDetail);
                            await _context.SaveChangesAsync();
                        }

                        await _context.SaveChangesAsync();

                        // Clear cart
                        HttpContext.Session.Remove("shoppingCart");

                        _notyfService.Success("Thank you for your order");

                        return RedirectToAction("Success", order);
                    }
                }
                catch {
                    ViewBag.shoppingCart = cart;

                    return View(confirmOrder);
                }

                return View(confirmOrder);
            }
            catch {
                _notyfService.Error("Need to fill information");
                return View(confirmOrder);
            }
            
        }

        // View of customer order successfully
        [Route("order-success.html", Name = "Order Successfully")]
        public IActionResult Success(Order order) {
            try {
                var accountId = HttpContext.Session.GetString("CustomerId");

                // Nếu chưa login => chuyển về trang login
                if (string.IsNullOrEmpty(accountId)) {
                    return RedirectToAction("Login", "UserAccount");
                }

                // Xác nhận xem đơn hàng đúng của customer đang đặt không
                var customer = _context.Customers.AsNoTracking()
                                                .SingleOrDefault(c => c.CustomerId == Convert.ToInt32(accountId));

                if (customer != null) {
                    return View(order);
                }
                
                return View();
            }
            catch {
                return View();
            }
        }
    }
}

