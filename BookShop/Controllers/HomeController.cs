using BookShop.Areas.Identity.Data;
using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookShop.Controllers
{
    public class HomeController : Controller
    {
        /*        private readonly ILogger<HomeController> _logger;
        */
        private BookShopContext _context;

        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<BookShopUser> _userManager;


        private readonly int maxofpage = 10;

        private readonly int rowsonepage = 4;

        /* public HomeController(ILogger<HomeController> logger)
         {
             _logger = logger;
         }*/
        public HomeController(BookShopContext context, ILogger<HomeController> logger, IEmailSender emailSender, UserManager<BookShopUser> userManager)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int id = 0, string searchString = "")
        {
            ViewData["CurrentFilter"] = searchString;
            var books = from s in _context.Books
                        select s;
            if (searchString != null)
            {
                books = books.Where(s => s.Title.Contains(searchString) || s.Category.Contains(searchString));
                /*            students = students.Where(s => s.LastName);
                */
            }
            int numOfFilteredStudent = books.Count();
            ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredStudent / rowsonepage);
            ViewBag.CurrentPage = id;
            List<Book> booklist = await books.Skip(id * rowsonepage)
                .Take(rowsonepage).ToListAsync();
            if (id > 0)
            {
                ViewBag.idpagprev = id - 1;
            }
            ViewBag.idpagenext = id + 1;
            ViewBag.currentPage = id;
            return View(booklist);
            /*            var books = await _context.Books.ToListAsync();
            */
        }

        [Authorize(Roles = "Seller")]
        public IActionResult ForSellerOnly()
        {

            /* ViewBag.message = "This is for Customer only! Hi " + _userManager.GetUserName(HttpContext.User);*/
            return View("Views/Store/Index.cshtml");
        }


        public IActionResult Privacy()
        {
            return View();
        }
        /*        public async Task<IActionResult> AddToCart(string isbn)
                {
                    string thisUserId = _userManager.GetUserId(HttpContext.User);
                    Cart myCart = new Cart() { UId = thisUserId, BookIsbn = isbn };
                    Cart fromDb = _context.Cart.FirstOrDefault(c => c.UId == thisUserId && c.BookIsbn == isbn);
                    //if not existing (or null), add it to cart. If already added to Cart before, ignore it.
                    if (fromDb == null)
                    {
                        _context.Add(myCart);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("List");
                }
        */

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
