using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookShop.Controllers
{
    public class StoreController : Controller
    {
        private readonly ILogger<StoreController> _logger;

        public StoreController(ILogger<StoreController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View("Views/Store/Index.cshtml");
        }

        public IActionResult Register()
        {
            return View("Views/Store/Register.cshtml");
        }
    }
}