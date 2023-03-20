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
        public int? libid;

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
            libid = id;
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
        public IActionResult Create(int? libraryId)
        {
            if (libraryId == null)
            {
                return NotFound();
            }

            var library = _context.Libraries.FirstOrDefault(l => l.Id == libraryId);

            if (library == null)
            {
                return NotFound();
            }

            ViewBag.Publishers = _context.Publishers.ToList();
            ViewBag.Styles = _context.Styles.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.LibraryId = libraryId;

            return RedirectToAction("Index", "Libraries");
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int libraryId, [Bind("Title,Year,PublisherId,StyleId,Pages,Annotation,Circulation")] Book book, int[] authorIds)
        {
            var library = _context.Libraries.FirstOrDefault(l => l.Id == libraryId);

            if (library == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var bookLibrary = new BookLibrary
                {
                    Book = book,
                    Library = library
                };

                _context.BookLibraries.Add(bookLibrary);

                foreach (var authorId in authorIds)
                {
                    var authorBook = new AuthorBook
                    {
                        AuthorId = authorId,
                        Book = book
                    };
                    _context.AuthorBooks.Add(authorBook);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Libraries", new { id = libraryId });
            }

            ViewBag.Publishers = _context.Publishers.ToList();
            ViewBag.Styles = _context.Styles.ToList();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.LibraryId = libraryId;

            return RedirectToAction("Index", "Libraries");
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
