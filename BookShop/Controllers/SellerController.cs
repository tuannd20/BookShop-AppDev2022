using BookShop.Areas.Identity.Data;
using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookShop.Controllers
{
    public class SellerController : Controller
    {
        private readonly ILogger<SellerController> _logger;
        private readonly UserManager<BookShopUser> _userManager;
        private BookShopContext _context;

        public SellerController(ILogger<SellerController> logger, UserManager<BookShopUser> userManager, BookShopContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Seller")]
        public IActionResult Index()
        {
            return View("Views/Store/Index.cshtml");
        }
        [Authorize(Roles = "Seller")]

        public IActionResult Register()
        {
            return View("Views/Store/Register.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addStore([Bind("UserId, Address, Slogan, Name")] Store store)
        {
            _context.Stores.Add(store);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Seller")]
        public IActionResult ForSellerOnly()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            Console.WriteLine(thisUserId);
            Store User = _context.Stores.FirstOrDefault(s => s.UserId == thisUserId);
            /* ViewBag.message = "This is for Customer only! Hi " + _userManager.GetUserName(HttpContext.User);*/
            Console.WriteLine("Store");
            if (User == null)
            {
                
                return View("Views/Store/Register.cshtml");
            }
            else
            {
                return View("Views/Book/Index.cshtml");
            }
           
        }
    }
}