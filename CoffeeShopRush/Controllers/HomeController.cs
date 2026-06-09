using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopRush.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // This is the entry point - shows splash screen first
    public IActionResult Index()
    {
        return View("Splash");
    }

    public IActionResult Landing()
    {
        // If already authenticated, go to game
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Play", "Game");
        }
        return View();
    }

    [Authorize]
    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}