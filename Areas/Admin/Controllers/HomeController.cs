using System.Diagnostics;
using E_Commerce.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public INotyfService _notyfService {get; }

    public HomeController(ILogger<HomeController> logger, INotyfService notyfService)
    {
        _logger = logger;
        _notyfService = notyfService;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        // _notyfService.Warning(Convert.ToString(User.Identity.IsAuthenticated));
        // _notyfService.Success(Convert.ToString(User.Identity.Name));
        var adminId = HttpContext.Session.GetString("AdminId");

        if (adminId != null) {
            return View();

        }

        return RedirectToAction("Login", "Authenticate");
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
