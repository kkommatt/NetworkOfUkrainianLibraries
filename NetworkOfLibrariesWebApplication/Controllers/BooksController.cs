using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetworkOfLibrariesWebApplication;
using Microsoft.Data.SqlClient;
using System.Net;

namespace NetworkOfLibrariesWebApplication.Controllers
{
    public class BooksController : Controller
    {
        private readonly DbnetworkOfLibrariesContext _context;
        private readonly string _connectionString;

        public BooksController(DbnetworkOfLibrariesContext context)
        {
            _context = context;
            _connectionString = "Server= (LocalDb)\\MSSQLLocalDB; Database=DBNetworkOfLibraries; Trusted_Connection=True; MultipleActiveResultSets=true";
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
            DbnetworkOfLibrariesContext.libid = id;
            var library = await _context.Libraries.Include(b => b.BookLibraries).ThenInclude(bl => bl.Book).ThenInclude(bl => bl.AuthorBooks).ThenInclude(bl => bl.Author).FirstOrDefaultAsync(book => book.Id == id);
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
                .Include(b => b.AuthorBooks)
                .ThenInclude(b => b.Author)
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
            libraryId = DbnetworkOfLibrariesContext.libid;
            if (libraryId == null)
            {
                return NotFound();
            }

            var library = _context.Libraries.FirstOrDefault(l => l.Id == libraryId);

            if (library == null)
            {
                return NotFound();
            }

            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Title");
            ViewData["StyleId"] = new SelectList(_context.Styles, "Id", "Name");
            ViewBag.AuthorId = new SelectList(_context.Authors, "Id", "Surname");
            ViewBag.LibraryId = libraryId;

            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? libraryId, [Bind("Title,Year,PublisherId,StyleId,Pages,Annotation,Circulation, AuthorId")] Book book)
        {
            int authorId = book.AuthorId.First();
            libraryId = DbnetworkOfLibrariesContext.libid;
            var library = await _context.Libraries.FirstOrDefaultAsync(l => l.Id == libraryId);
            book.Style = await _context.Styles.FirstOrDefaultAsync(style => style.Id == book.StyleId);
            book.Publisher = await _context.Publishers.FirstOrDefaultAsync(publisher => publisher.Id == book.PublisherId);
            if (library == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                library.Books.Add(book);
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                        string insertBookLibraryQuery = "INSERT INTO BookLibraries (BookId, LibraryId) " +
                                                        "VALUES (@BookId, @LibraryId)";
                        using (SqlCommand command = new SqlCommand(insertBookLibraryQuery, connection))
                        {
                            command.Parameters.AddWithValue("@BookId", book.Id);
                            command.Parameters.AddWithValue("@LibraryId", libraryId);

                            await command.ExecuteNonQueryAsync();
                        }
                    

                    // Add book to authors

                        string insertAuthorBookQuery = "INSERT INTO AuthorBooks (AuthorId, BookId) " +
                                                        "VALUES (@AuthorId, @BookId)";
                        using (SqlCommand command = new SqlCommand(insertAuthorBookQuery, connection))
                        {
                            command.Parameters.AddWithValue("@AuthorId", authorId);
                            command.Parameters.AddWithValue("@BookId", book.Id);

                            await command.ExecuteNonQueryAsync();
                        }
                }


                
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Libraries", new { id = libraryId });
            }

            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Title", book.PublisherId);
            ViewData["StyleId"] = new SelectList(_context.Styles, "Id", "Name", book.StyleId);
            ViewBag.AuthorId = new SelectList(_context.Authors, "Id", "Surname");
            ViewBag.LibraryId = libraryId;

            return RedirectToAction("Details", "Libraries", new { id = libraryId });
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
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Title", book.PublisherId);
            ViewData["StyleId"] = new SelectList(_context.Styles, "Id", "Name", book.StyleId);
            ViewBag.AuthorId = new SelectList(_context.Authors, "Id", "Surname");
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,PublisherId,StyleId,Pages,Annotation,Circulation, AuthorId")] Book book)
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
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.OpenAsync();
                        string updateAuthorBookQuery = "UPDATE AuthorBooks SET AuthorId = @AuthorId WHERE BookId = @BookId";
                        using (SqlCommand command = new SqlCommand(updateAuthorBookQuery, connection))
                        {
                            command.Parameters.AddWithValue("@AuthorId", book.AuthorId.First());
                            command.Parameters.AddWithValue("@BookId", book.Id);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
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
                return RedirectToAction("Details", "Libraries", new { id = DbnetworkOfLibrariesContext.libid });
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Title", book.PublisherId);
            ViewData["StyleId"] = new SelectList(_context.Styles, "Id", "Name", book.StyleId);
            ViewBag.AuthorId = new SelectList(_context.Authors, "Id", "Surname");
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
                .Include(b => b.AuthorBooks)
                .ThenInclude(b => b.Author)
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string deleteAuthorBookQuery = "DELETE FROM AuthorBooks WHERE BookId = @BookId";
                    using (SqlCommand command = new SqlCommand(deleteAuthorBookQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BookId", id);
                        await command.ExecuteNonQueryAsync();
                    }

                    string deleteBookLibrary = "DELETE FROM BookLibraries WHERE BookId = @BookId";
                    using (SqlCommand command = new SqlCommand(deleteBookLibrary, connection))
                    {
                        command.Parameters.AddWithValue("@BookId", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Libraries", new { id = DbnetworkOfLibrariesContext.libid });
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
