using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Extension;
using E_Commerce.Models;
using E_Commerce.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers {
    public class ShoppingCartController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public ShoppingCartController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        // Khởi tạo cart của user
        public List<CartItemViewModel> shoppingCart {
            // getter
            get {
                var cart = HttpContext.Session.Get<List<CartItemViewModel>>("shoppingCart");

                // Xử lý trường hợp cart == null
                if (cart == default(List<CartItemViewModel>)) {
                    cart = new List<CartItemViewModel>();
                }

                return cart;
            }
        }

        [HttpPost]
        [Route("api/v1/cart/add")]
        public IActionResult addToCart(int productId, int? quantity) {
            List<CartItemViewModel> cart = shoppingCart;

            try {
                // Add product
                CartItemViewModel item = cart.SingleOrDefault(p => p.product.ProductId == productId);

                // Đã có trong cart => update quantity
                if (item != null) {
                    item.quantity = item.quantity + quantity.Value;
                }
                // chưa có trong cart
                else {
                    Product productAdded = _context.Products.SingleOrDefault(p => p.ProductId == productId);

                    item = new CartItemViewModel {
                        quantity = quantity.HasValue ? quantity.Value : 1,
                        product = productAdded
                    };

                    // Update cart
                    cart.Add(item);
                }

                // Save session
                HttpContext.Session.Set<List<CartItemViewModel>>("shoppingCart", cart);

                _notyfService.Success("Add to cart successfully");

                return Json(new {success = true});
            }
            catch {
                return Json(new {success = false});
            }
        }

        [HttpPost]
        [Route("api/v1/cart/update")]
        public IActionResult updateCart(int productId, int? quantity) {
            // Lấy cart để xử lí
            var cart = HttpContext.Session.Get<List<CartItemViewModel>>("shoppingCart");

            try {
                if (cart != null) {
                    CartItemViewModel item = cart.SingleOrDefault(c => c.product.ProductId == productId);

                    // product có sẵn trong cart -> update quantity
                    if (item != null) {
                        item.quantity = quantity.Value;
                    }

                    // Save session
                    HttpContext.Session.Set<List<CartItemViewModel>>("shoppingCart", cart);
                }
                return Json(new {success = true});
            }
            catch {
                return Json(new {success = false});
            }
        }

        [HttpPost]
        [Route("api/v1/cart/remove")]
        public IActionResult removeProduct(int productId) {
            try {
                List<CartItemViewModel> cart = shoppingCart;
                CartItemViewModel item = cart.SingleOrDefault(c => c.product.ProductId == productId);

                if (item != null) {
                    cart.Remove(item);
                }

                // Save session
                HttpContext.Session.Set<List<CartItemViewModel>>("shoppingCart", cart);

                return Json(new {success = true});
            }
            catch {
                return Json(new {success = false});
            }

            
        }

        [Route("cart.html", Name="Shopping Cart")]
        public IActionResult Index() 
        {
            return View(shoppingCart);
        }
    }
}