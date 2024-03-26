using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers {
    public class ProductController : Controller {
        private readonly EcommerceContext _context;
        public INotyfService _notyfService {get; }

        public ProductController(EcommerceContext context, INotyfService notyfService) {
            _context = context;
            _notyfService = notyfService;
        }

        public IActionResult Index() 
        {
            return View();
        }

        public IActionResult Details() 
        {
            return View();
        }

        // public async Task<IActionResult> Details(int? id) {
        //     if (id == null) {
        //         return NotFound();
        //     }

        //     var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

        //     if (product == null) {
        //         return NotFound();
        //     }

        //     return View(product);
        // }
    }
}