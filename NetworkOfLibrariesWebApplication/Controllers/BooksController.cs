using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetworkOfLibrariesWebApplication;

namespace NetworkOfLibrariesWebApplication.Controllers
{
    public class BooksController : Controller
    {
        private readonly DbnetworkOfLibrariesContext _context;

        public BooksController(DbnetworkOfLibrariesContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(int? id, string? name, string? adress, string? website, string? schedule, int? cityid)
        {
            if (id == null) return RedirectToAction("Libraries", "Index");
            ViewBag.LibraryId = id;
            ViewBag.LibraryName = name;
            ViewBag.LibraryAdress = adress;
            ViewBag.LibraryWebsite = website;
            ViewBag.LibrarySchedule = schedule;
            ViewBag.LibraryCityid = cityid;
            var library = await _context.Libraries.Include(b => b.BookLibraries).ThenInclude(bl => bl.Book).FirstOrDefaultAsync(book => book.Id == id);
            if(library is null)
                return RedirectToAction("Libraries", "Index");
            var booksByLibrary = library.BookLibraries.Select(bookLibrary => bookLibrary.Book);
            return View(booksByLibrary.ToList());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Style)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id");
            ViewData["StyleId"] = new SelectList(_context.Styles, "Id", "Id");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ICollection<Author>? Libraries, [Bind("Id,Title,Year,PublisherId,StyleId,Pages,Annotation,Circulation")] Book book)
        {
            var id =  _context.Libraries.Include(b => b.BookLibraries).ThenInclude(bl => bl.Book).Select(book => book.Id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Books", new
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id", book.PublisherId);
            ViewData["StyleId"] = new SelectList(_context.Styles, "Id", "Id", book.StyleId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id", book.PublisherId);
            ViewData["StyleId"] = new SelectList(_context.Styles, "Id", "Id", book.StyleId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,PublisherId,StyleId,Pages,Annotation,Circulation")] Book book)
        {
            if (id != book.Id)
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
                    if (!BookExists(book.Id))
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
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Id", book.PublisherId);
            ViewData["StyleId"] = new SelectList(_context.Styles, "Id", "Id", book.StyleId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .Include(b => b.Style)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'DbnetworkOfLibrariesContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
