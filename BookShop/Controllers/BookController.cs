#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Areas.Identity.Data;
using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace BookShop.Controllers
{
    public class BookController : Controller
    {
        private readonly BookShopContext _context;

        private readonly UserManager<BookShopUser> _userManager;


        private readonly int maxofpage = 10;

        private readonly int rowsonepage = 4;

        public BookController(BookShopContext context, UserManager<BookShopUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Book
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Index(int id = 0, string searchString = "")
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            ViewData["CurrentFilter"] = searchString;
            var books = from s in _context.Books
                           select s;
            books = books.Include(s => s.Store).ThenInclude(u => u.User)
                .Where(u => u.Store.User.Id == userid);
            if (searchString != null)
            {
                books = books.Include(s => s.Store).ThenInclude(u => u.User)
                .Where(u => u.Store.User.Id == userid)
                .Where(s => s.Title.Contains(searchString) || s.Category.Contains(searchString));
            }
            int numOfFilteredStudent = books.Count();
            ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredStudent / rowsonepage);
            ViewBag.CurrentPage = id;
            List<Book> studentsList = await books.Skip(id * rowsonepage)
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
            if (searchString != null)
            {
                books = books.Where(s => s.Title.Contains(searchString) || s.Category.Contains(searchString));
                /*            students = students.Where(s => s.LastName);
                */
            }
            int numOfFilteredBook = books.Count();
            ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredBook / rowsonepage);
            ViewBag.CurrentPage = id;
            List<Book> booklist = await books.Skip(id * rowsonepage)
                .Take(rowsonepage).ToListAsync();
            if (id > 0)
            {
                ViewBag.idpagprev = id - 1;
            }
            ViewBag.idpagenext = id + 1;
            ViewBag.currentPage = id;
            return View("Views/Book/Search.cshtml", booklist);
        }

        public async Task<IActionResult> DisplayBook(string Isbn)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Isbn == Isbn);
            return View("Views/Book/BookDetails.cshtml", book);
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
            var userid = _userManager.GetUserId(HttpContext.User);


            ViewData["StoreId"] = _context.Stores.Where(s => s.UserId == userid).FirstOrDefault().Name;        
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
            var userid = _userManager.GetUserId(HttpContext.User);


            ViewData["StoreId"] = _context.Stores.Where(s => s.UserId == userid).FirstOrDefault().Name;

            try
            {
                if (ModelState.IsValid)
                {
                    var userid1 = _userManager.GetUserId(HttpContext.User);
                    string imgName = book.Isbn + Path.GetExtension(image.FileName);
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }
                    book.ImgUrl = imgName;

                    Store thisStore = _context.Stores.Where(s => s.UserId == userid1).FirstOrDefault();
                    book.StoreId = thisStore.Id;
                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                TempData["msg"] = "<script>alert('You are seller. Can't get in here.');</script>";
             
            }
           
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
        public async Task<IActionResult> Edit([Bind("Isbn,StoreId,Title,Pages,Author,Category,Price,Desc,ImgUrl")] Book book, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    if(image == null)
                    {
                        Book thisProduct = _context.Books.Where(p => p.Isbn == book.Isbn).AsNoTracking().FirstOrDefault();
                        book.ImgUrl = thisProduct.ImgUrl;

                    }
                    else
                    {
                        string imgName = book.Isbn + Path.GetExtension(image.FileName);
                        string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                        using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        book.ImgUrl = imgName;
                    }
                    var userid1 = _userManager.GetUserId(HttpContext.User);
                    Store thisStore = _context.Stores.Where(s => s.UserId == userid1).FirstOrDefault();
                    book.StoreId = thisStore.Id;
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
