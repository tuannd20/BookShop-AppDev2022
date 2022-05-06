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
using Microsoft.AspNetCore.Identity;
using BookShop.Areas.Identity.Data;

namespace BookShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly BookShopContext _context;
        private readonly UserManager<BookShopUser> _userManager;

        public OrderController(BookShopContext context, UserManager<BookShopUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            int total = 0;
            var bookShops = _context.Carts.Include(c => c.Book)
                                                .Include(c => c.User)
                                                .Where(u => u.UserId == userid);
            
            List<Cart> DetailsCart = await _context.Carts
                .Where(c => c.UserId == userid)
                .Include(c => c.Book)
                .ToListAsync();
            for(int i = 0; i < DetailsCart.Count; i++)
            {
                total += ((int)DetailsCart[i].Quantity * DetailsCart[i].Book.Price);
            }
            ViewData["Total"] = total;
            return View(await bookShops.ToListAsync());
        }

        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task<IActionResult> Checkout(int total)
        {
            string userid = _userManager.GetUserId(HttpContext.User);
            List<Cart> DetailsCart = await _context.Carts
                .Where(c => c.UserId == userid)
                .Include(c => c.Book)
                .ToListAsync();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Step 1: create an order
                    Order myOrder = new Order();
                    myOrder.UserId = userid;
                    myOrder.OrderDate = DateTime.Now;
                    myOrder.Total = total;
                    _context.Add(myOrder);
                    await _context.SaveChangesAsync();

                    //Step 2: insert all order details by var "myDetailsInCart"
                    foreach (var item in DetailsCart)
                    {
                        OrderDetail detail = new OrderDetail()
                        {
                            OrderId = myOrder.Id,
                            BookIsbn = item.BookIsbn,
                            Quantity = (int) item.Quantity
                        };
                        _context.Add(detail);
                    }
                    await _context.SaveChangesAsync();

                    //Step 3: empty/delete the cart we just done for thisUser
                    _context.Carts.RemoveRange(DetailsCart);
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

        public async Task<IActionResult> RecordOrder()
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var recordOrder = await _context.Books.Include(o => o.OrderDetails).ThenInclude(od => od.Order)
                           .Where(od => od.Store.UserId == userid).ToListAsync();


            return View(recordOrder);
        }
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
