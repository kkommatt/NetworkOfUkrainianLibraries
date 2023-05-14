using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.Data.SqlClient;
using NetworkOfLibrariesWebApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetworkOfLibrariesWebApplication;

namespace NetworkOfLibrariesWebApplication.Infrastructure.Services
{
    public interface IImportService<TEntity> where TEntity : Entity
    {
        Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
    }

    public interface IExportService<TEntity>
 where TEntity : Entity
    {
        Task WriteToAsync(Stream stream, CancellationToken
       cancellationToken);
    }

    public interface IDataPortServiceFactory<TEntity> where TEntity : Entity
    {
        IImportService<TEntity> GetImportService(string contentType);
        IExportService<TEntity> GetExportService(string contentType);
    }
    public class LibraryImportService : IImportService<Library>
    {
        private readonly DbnetworkOfLibrariesContext context;
        private readonly string _connectionString;

        public LibraryImportService(DbnetworkOfLibrariesContext context)
        {
            this.context = context;
            _connectionString = "Server= (LocalDb)\\MSSQLLocalDB; Database=DBNetworkOfLibraries; Trusted_Connection=True; MultipleActiveResultSets=true";
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream is not readable", nameof(stream)); 
            }
            using var workBook = new XLWorkbook(stream);
            var worksheet = workBook.Worksheets.FirstOrDefault();
            if (worksheet is null)
            {
                return;
            }
            foreach (var rows in worksheet.RowsUsed().Skip(1))
            {
                await AddLibraryAsync(rows, cancellationToken);
            }
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddLibraryAsync(IXLRow row, CancellationToken cancellationToken)
        {
            try
            {
                var libraryName = row.Cell(12).GetValue<string>();
                Library newLibrary;
                var libs = (from lib in context.Libraries
                            where lib.Name.Contains(libraryName)
                            select lib).ToList();
                if (libs.Count > 0)
                {
                    newLibrary = libs[0];
                }
                else
                {
                    newLibrary = new Library();
                    newLibrary.Name = libraryName;
                    newLibrary.Adress = row.Cell(13).Value.ToString();
                    newLibrary.Website = row.Cell(14).Value.ToString();
                    newLibrary.Schedule = row.Cell(15).Value.ToString();
                    newLibrary.CityId = (int)row.Cell(16).Value;

                    await context.Libraries.AddAsync(newLibrary);
                }
                Book book = new Book();
                book.Title = row.Cell(1).Value.ToString();
                book.Year = (int)row.Cell(6).Value;
                book.PublisherId = (int)row.Cell(7).Value;
                book.StyleId = (int)row.Cell(8).Value;
                book.Pages = (int)row.Cell(9).Value;
                book.Annotation = row.Cell(10).Value.ToString();
                book.Circulation = (int)row.Cell(11).Value;

                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();
                Author author;

                var a = (from aut in context.Authors
                         where aut.Surname.Contains(row.Cell(2).Value.ToString())
                         select aut).ToList();
                if (a.Count > 0)
                {
                    author = a[0];
                }
                else
                {
                    author = new Author();
                    author.Surname = row.Cell(2).Value.ToString();
                    author.FirstName = row.Cell(3).Value.ToString();
                    author.Datebirth = row.Cell(4).Value.ToString();
                    author.Education = row.Cell(5).Value.ToString();
                    context.Authors.Add(author);
                    await context.SaveChangesAsync();
                }
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string insertBookLibraryQuery = "INSERT INTO BookLibraries (BookId, LibraryId) " +
                                                    "VALUES (@BookId, @LibraryId)";
                    using (SqlCommand command = new SqlCommand(insertBookLibraryQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BookId", book.Id);
                        command.Parameters.AddWithValue("@LibraryId", newLibrary.Id);

                        await command.ExecuteNonQueryAsync();
                    }
                    string insertAuthorBookQuery = "INSERT INTO AuthorBooks (AuthorId, BookId) " +
                                                    "VALUES (@AuthorId, @BookId)";
                    using (SqlCommand command = new SqlCommand(insertAuthorBookQuery, connection))
                    {
                        command.Parameters.AddWithValue("@AuthorId", author.Id);
                        command.Parameters.AddWithValue("@BookId", book.Id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                await context.SaveChangesAsync();
            }
            catch
            {
                throw new ArgumentException("Input stream is not writable"); 
            }
        }
    }
    public class LibraryExportService : IExportService<Library>
    {
        private const string RootWorksheetName = "Libraries";
        private static readonly IReadOnlyList<string> HeaderNames =
        new string[]
        {
            "Назва",
            "Адреса",
            "Веб-сайт",
            "Розклад",
            "Місто",
        };
        private readonly DbnetworkOfLibrariesContext context;

        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex <
           HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value =
               HeaderNames[columnIndex];
            }
            worksheet.Row(1).Style.Font.Bold = true;
        }
        private static void WriteLibrary(IXLWorksheet worksheet, Library library, int rowIndex)
        {
            var columnIndex = 1;
            worksheet.Cell(rowIndex, columnIndex++).Value = library.Name;
            worksheet.Cell(rowIndex, columnIndex++).Value = library.Adress;
            worksheet.Cell(rowIndex, columnIndex++).Value = library.Website;
            worksheet.Cell(rowIndex, columnIndex++).Value = library.Schedule;
            worksheet.Cell(rowIndex, columnIndex++).Value = library.City.Name;
        }

        private static void WriteLibraries(IXLWorksheet worksheet, ICollection<Library> libraries)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var library in libraries)
            {
                WriteLibrary(worksheet, library, rowIndex);
                rowIndex += 1;
            }
        }

        public LibraryExportService(DbnetworkOfLibrariesContext context)
        {
            this.context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Input stream is not writable"); 
            }
            var libraries = await context.Libraries.Include(library => library.City).ToListAsync(cancellationToken);
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(RootWorksheetName);
            WriteLibraries(worksheet, libraries);
            workbook.SaveAs(stream);
        }
    }
}


