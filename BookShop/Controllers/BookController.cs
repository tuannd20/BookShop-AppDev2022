#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace BookShop.Controllers
{
    public class BookController : Controller
    {
        private readonly BookShopContext _context;

        private readonly int maxofpage = 10;

        private readonly int rowsonepage = 4;


        public BookController(BookShopContext context)
        {
            _context = context;
        }

        // GET: Book
        public async Task<IActionResult> Index(int id = 0, string searchString = "")
        {
            ViewData["CurrentFilter"] = searchString;
            var students = from s in _context.Books
                           select s;
            students = students.Where(s => s.Title.Contains(searchString) || s.Category.Contains(searchString));
            /*            students = students.Where(s => s.LastName);
            */
            int numOfFilteredStudent = students.Count();
            ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredStudent / rowsonepage);
            ViewBag.CurrentPage = id;
            List<Book> studentsList = await students.Skip(id * rowsonepage)
                .Take(rowsonepage).ToListAsync();
            if (id > 0)
            {
                ViewBag.idpagprev = id - 1;
            }
            ViewBag.idpagenext = id + 1;
            ViewBag.currentPage = id;
            return View(studentsList);
        }

        public async Task<IActionResult> SearchBook(int id = 0, string searchString = "")
        {
            ViewData["CurrentFilter"] = searchString;
            var books = from s in _context.Books
                           select s;
            books = books.Where(s => s.Title.Contains(searchString) || s.Category.Contains(searchString));
            List<Book> booksList = await books.Skip(id * rowsonepage)
               .Take(rowsonepage).ToListAsync();
            return View("Views/Book/Search.cshtml", booksList);
        }


        // GET: Book/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public async Task<IActionResult> Create()
        {
           
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Id");
            return View();
        }   
        public async Task<IActionResult> RecordOrder()
        {
          
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,StoreId,Category,Title,Pages,Author,Price,Desc,ImgUrl")] Book book, IFormFile image)
        {
            if(ModelState.IsValid)
            {
                string imgName = book.Isbn + Path.GetExtension(image.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                using(var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.ImgUrl = imgName;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Id", book.StoreId);
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Id", book.StoreId);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,StoreId,Title,Pages,Author,Category,Price,Desc,ImgUrl")] Book book)
        {
            if (id != book.Isbn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Isbn))
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
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Id", book.StoreId);
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return _context.Books.Any(e => e.Isbn == id);
        }
    }
}
