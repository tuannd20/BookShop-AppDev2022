#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShop.Data;
using BookShop.Models;
using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace BookShop.Controllers
{
    public class CartController : Controller
    {
        private readonly BookShopContext _context;
        private readonly UserManager<BookShopUser> _userManager;

        public CartController(BookShopContext context, UserManager<BookShopUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);

            var bookShopContext = _context.Carts.Include(c => c.Book)
                                                .Include(c => c.User)
                                                .Where(u => u.UserId == userid);

            return View(await bookShopContext.ToListAsync());
        }

        // GET: Cart/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Cart/Create
        public IActionResult Create()
        {
            ViewData["BookIsbn"] = new SelectList(_context.Books, "Isbn", "Isbn");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,BookIsbn")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookIsbn"] = new SelectList(_context.Books, "Isbn", "Isbn", cart.BookIsbn);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);
            return View(cart);
        }

        // GET: Cart/Edit/5
        public async Task<IActionResult> Edit(string uid, string bid)
        {
            if (bid == null || uid == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(m => m.UserId == uid && m.BookIsbn == bid);
            return View(cart);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("UserId,BookIsbn,Quantity")] Cart cart, int quantity)
        {
            try
            {
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Edit", new { uid = cart.UserId, bid = cart.BookIsbn });
            }
        }

        public async Task<IActionResult> Remove(string cartId)
        {
            var userid = _userManager.GetUserId(HttpContext.User);

            var cart = _context.Carts.Where(s => s.UserId == userid).FirstOrDefault();
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddToCart(int? quantity, string isbn)
        {
            try
            {

                var thisUserId = _userManager.GetUserId(HttpContext.User);
                if (thisUserId == null)
                {
                    return RedirectToAction("Login", "Identity");
                }
                Store thisStore = _context.Stores.FirstOrDefault(s => s.UserId == thisUserId);
                if (quantity == null)
                {
                    quantity = 1;
                }
                if (thisStore != null)
                {
                    TempData["msg"] = "<script>alert('You are seller. Can't get in here.');</script>";
                    return RedirectToAction("Index");

                }
                else
                {
                    Cart myCart = new Cart() { UserId = thisUserId, BookIsbn = isbn, Quantity = quantity };
                    Cart fromDb = _context.Carts.FirstOrDefault(c => c.UserId == thisUserId && c.BookIsbn == isbn);
                    //if not existing (or null), add it to cart. If already added to Cart before, ignore it.
                    if (fromDb == null)
                    {
                        _context.Add(myCart);
                        await _context.SaveChangesAsync();

                    }
                    return RedirectToAction("Index");

                }
            }
            catch (InvalidOperationException)
            {
                TempData["msg"] = "<script>alert('You are seller. Can't get in here.');</script>";
                return RedirectToAction("SearchBook", "Book");
            }

        }

        public async Task<IActionResult> Checkout()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            List<Cart> myDetailsInCart = await _context.Carts
                .Where(c => c.UserId == thisUserId)
                .Include(c => c.Book)
                .ToListAsync();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Step 1: create an order
                    Order myOrder = new Order();
                    myOrder.UserId = thisUserId;
                    myOrder.OrderDate = DateTime.Now;
                    var Total = 0;

                    for (int i = 0; i < myDetailsInCart.Count; i++)
                    {
                        Total +=  Total + (myDetailsInCart[i].Book.Price *(int) myDetailsInCart[i].Quantity);

                    }

                    myOrder.Total = Total;
                    _context.Add(myOrder);
                    await _context.SaveChangesAsync();

                    //Step 2: insert all order details by var "myDetailsInCart"
                    foreach (var item in myDetailsInCart)
                    {
                        OrderDetail detail = new OrderDetail()
                        {
                            OrderId = myOrder.Id,
                            BookIsbn = item.BookIsbn,
                            Quantity = item.Quantity
                        };
                        _context.Add(detail);
                    }
                    await _context.SaveChangesAsync();

                    //Step 3: empty/delete the cart we just done for thisUser
                    _context.Carts.RemoveRange(myDetailsInCart);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error occurred in Checkout" + ex);
                }
            }

            return RedirectToAction("Index", "Cart");
        }

        private bool CartExists(string id)
        {
            return _context.Carts.Any(e => e.UserId == id);
        }
    }
}
