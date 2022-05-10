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

namespace BookShop.Controllers
{
    public class CartTestController : Controller
    {
        private readonly BookShopContext _context;

        public CartTestController(BookShopContext context)
        {
            _context = context;
        }

        // GET: CartTest
        public async Task<IActionResult> Index()
        {
            var bookShopContext = _context.Carts.Include(c => c.Book).Include(c => c.User);
            return View(await bookShopContext.ToListAsync());
        }

        // GET: CartTest/Details/5
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

        // GET: CartTest/Create
        public IActionResult Create()
        {
            ViewData["BookIsbn"] = new SelectList(_context.Books, "Isbn", "Isbn");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CartTest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,BookIsbn,Quantity")] Cart cart)
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

        // GET: CartTest/Edit/5
        public async Task<IActionResult> Edit(string uid ,string bid)
        {
            if (bid == null || uid == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(m => m.UserId == uid && m.BookIsbn == bid);
          
            return View(cart);
        }

        // POST: CartTest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,BookIsbn,Quantity")] Cart cart, int quantity)
        {
            /*if (id != cart.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.UserId))
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
            ViewData["BookIsbn"] = new SelectList(_context.Books, "Isbn", "Isbn", cart.BookIsbn);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);*/
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

        // GET: CartTest/Delete/5
        public async Task<IActionResult> Delete(string id)
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

        // POST: CartTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cart = await _context.Carts.FindAsync(id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(string id)
        {
            return _context.Carts.Any(e => e.UserId == id);
        }
    }
}
