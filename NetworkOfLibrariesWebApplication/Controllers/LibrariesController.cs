using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using ClosedXML.Excel.CalcEngine;
using ClosedXML.Graphics;
using static ClosedXML.Excel.XLProtectionAlgorithm;
using NetworkOfLibrariesWebApplication;
using Microsoft.Data.SqlClient;
using NetworkOfLibrariesWebApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using NetworkOfLibrariesWebApplication.Infrastructure.Identity.Extensions;

namespace NetworkOfLibrariesWebApplication.Controllers
{
    public class LibrariesController : Controller
    {
        private readonly DbnetworkOfLibrariesContext _context;
        private readonly string _connectionString;

        public LibrariesController(DbnetworkOfLibrariesContext context)
        {
            _context = context;
            _connectionString = "Server= (LocalDb)\\MSSQLLocalDB; Database=DBNetworkOfLibraries; Trusted_Connection=True; MultipleActiveResultSets=true";
        }

        // GET: Libraries
        public async Task<IActionResult> Index()
        {
            var dbnetworkOfLibrariesContext = _context.Libraries.Include(l => l.City);
            return View(await dbnetworkOfLibrariesContext.ToListAsync());
        }

        // GET: Libraries/Details/5
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Libraries == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries
                .Include(bookLibrary => bookLibrary.BookLibraries)
                .ThenInclude(b => b.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (library == null)
            {
                return NotFound();
            }

            //return View(library);
            return RedirectToAction("Index", "Books", new { id = library.Id, name = library.Name, adress = library.Adress, website = library.Website, schedule = library.Schedule, cityid = library.CityId });
        }

        // GET: Libraries/Create
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id");
            return View();
        }

        // POST: Libraries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Create([Bind("Id,Name,Adress,Website,Schedule,CityId")] Library library)
        {
            if (ModelState.IsValid)
            {
                _context.Add(library);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", library.CityId);
            return View(library);
        }

        // GET: Libraries/Edit/5
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Libraries == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries.FindAsync(id);
            if (library == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", library.CityId);
            return View(library);
        }

        // POST: Libraries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Adress,Website,Schedule,CityId")] Library library)
        {
            if (id != library.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(library);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryExists(library.Id))
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
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Id", library.CityId);
            return View(library);
        }

        // GET: Libraries/Delete/5
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Libraries == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries
                .Include(l => l.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (library == null)
            {
                return NotFound();
            }

            return View(library);
        }

        // POST: Libraries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Libraries == null)
            {
                return Problem("Entity set 'DbnetworkOfLibrariesContext.Libraries'  is null.");
            }
            var library = await _context.Libraries.FindAsync(id);
            if (library != null)
            {
                _context.Libraries.Remove(library);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryExists(int id)
        {
            return (_context.Libraries?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public IActionResult Import()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = RoleNames.Admin)]
        public async Task<IActionResult> Import(IFormFile librariesFile, CancellationToken cancellationToken)
        {
            LibraryDataPortServiceFactory factorylib = new LibraryDataPortServiceFactory(_context);
            var importService = factorylib.GetImportService(librariesFile.ContentType);
            using var stream = librariesFile.OpenReadStream();
            await importService.ImportFromStreamAsync(stream, cancellationToken);
            return RedirectToAction("");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Export([FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", CancellationToken cancellationToken = default)
        {
            LibraryDataPortServiceFactory factorylib = new LibraryDataPortServiceFactory(_context);
            var exportService = factorylib.GetExportService(contentType);
            var memoryStream = new MemoryStream();
            await exportService.WriteToAsync(memoryStream, cancellationToken);
            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;
            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName =$"libraries_{DateTime.UtcNow.ToShortDateString()}.xlsx"
            };
        }
    }
}

    


