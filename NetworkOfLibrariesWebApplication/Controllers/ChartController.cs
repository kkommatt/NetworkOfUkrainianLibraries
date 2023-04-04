using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NetworkOfLibrariesWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly DbnetworkOfLibrariesContext _context;

        public ChartController(DbnetworkOfLibrariesContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var libraries = _context.Libraries.ToList();
            List<object> libBook = new List<object>();
            libBook.Add(new[] { "Бібліотека", "Кількість книг" });
            foreach (var library in libraries)
            {

                libBook.Add(new object[] { library.Name, _context.BookLibraries.Where(bl => bl.LibraryId == library.Id).Select(b => b.BookId).Count() });
            }
            return new JsonResult(libBook);
        }
        [HttpGet("JsonData1")]
        public JsonResult JsonData1()
        {
            var cities = _context.Cities.ToList();
            List<object> cityLibrary = new List<object>();
            cityLibrary.Add(new[] { "Місто", "Кількість бібліотек" });
            foreach (var city in cities)
            {

                cityLibrary.Add(new object[] { city.Name, _context.Libraries.Where(library => library.CityId == city.Id).Count()});
            }
            return new JsonResult(cityLibrary);
        }
    }
}
