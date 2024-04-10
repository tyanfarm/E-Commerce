using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using E_Commerce.Models;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace E_Commerce.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public INotyfService _notyfService {get; }

    public HomeController(ILogger<HomeController> logger, INotyfService notyfService)
    {
        _logger = logger;
        _notyfService = notyfService;
    }

    public IActionResult Index()
    {
        // _notyfService.Warning(Convert.ToString(User.Identity.IsAuthenticated));
        // _notyfService.Success(Convert.ToString(User.Identity.Name));

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
