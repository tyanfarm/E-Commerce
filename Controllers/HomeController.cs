using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using E_Commerce.Models;
using E_Commerce.ModelViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace E_Commerce.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly EcommerceContext _context;

    public HomeController(ILogger<HomeController> logger, EcommerceContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        HomeVM model = new HomeVM();

        var listProducts = _context.Products.AsNoTracking()
                                            .Where(p => p.Active == true && p.HomeFlag == true)
                                            .OrderByDescending(p => p.ProductId)
                                            .ToList();

        List<ProductHomeVM> listProductViews = new List<ProductHomeVM>();

        var listCategories = _context.Categories.AsNoTracking()
                                                .Where(c => c.Published == true)
                                                .OrderByDescending(c => c.CatId)
                                                .ToList();

        foreach(var category in listCategories) {   
            ProductHomeVM productHome = new ProductHomeVM();

            productHome.category = category;
            productHome.listProducts = listProducts.Where(p => p.CatId == category.CatId).ToList();

            listProductViews.Add(productHome);
        }
        
        model.Products = listProductViews;

        ViewBag.allProducts = listProducts;

        return View(model);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
