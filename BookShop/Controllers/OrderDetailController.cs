
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
    public class OrderDetailController : Controller
    {
        private readonly BookShopContext _context;
        private readonly UserManager<BookShopUser> _userManager;

        public OrderDetailController(BookShopContext context, UserManager<BookShopUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: OrderDetail
        public async Task<IActionResult> Index(int? id)
        {

            var userid = _userManager.GetUserId(HttpContext.User);

            var orderDetail = from b in _context.OrderDetails select b;

            orderDetail = orderDetail.Include(u => u.Order).Include(b => b.Book).ThenInclude(s => s.Store)
                .Where(f => f.OrderId == id);
                /*.Where(v => v.Order.UserId == userid);*/
           /* List<Order> ordersList = await ordered.Skip(id * _recordsPerPage)
                .Take(_recordsPerPage).ToListAsync();*/

            return View(orderDetail);
        }

        // GET: OrderDetail/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Book)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetail/Create
        public IActionResult Create()
        {
            ViewData["BookIsbn"] = new SelectList(_context.Books, "Isbn", "Isbn");
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            return View();
        }

        // POST: OrderDetail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,BookIsbn,Quantity")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookIsbn"] = new SelectList(_context.Books, "Isbn", "Isbn", orderDetail.BookIsbn);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
            return View(orderDetail);
        }

        // GET: OrderDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["BookIsbn"] = new SelectList(_context.Books, "Isbn", "Isbn", orderDetail.BookIsbn);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
            return View(orderDetail);
        }

        // POST: OrderDetail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,BookIsbn,Quantity")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookIsbn"] = new SelectList(_context.Books, "Isbn", "Isbn", orderDetail.BookIsbn);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetail.OrderId);
            return View(orderDetail);
        }

        // GET: OrderDetail/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Book)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderId == id);
        }
    }
}
