using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookShop.Controllers
{
    public class StoreController : Controller
    {
        private readonly BookShopContext _db;

        public StoreController(BookShopContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
			
			IEnumerable<Store> objStore = _db.Store.ToList();
			Console.WriteLine("Store:", objStore);
			return View("Views/Store/Index.cshtml", objStore);
        }

        public IActionResult Register()
        {
            return View("Views/Store/Register.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addStore([Bind("UserId, Address, Slogan, Name")] Store store)
        {
            _db.Store.Add(store);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}