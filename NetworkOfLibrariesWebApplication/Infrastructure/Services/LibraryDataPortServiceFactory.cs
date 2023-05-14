namespace NetworkOfLibrariesWebApplication.Infrastructure.Services
{
    public class LibraryDataPortServiceFactory : IDataPortServiceFactory<Library>
    {
        private readonly DbnetworkOfLibrariesContext libraryContext;
        public LibraryDataPortServiceFactory(DbnetworkOfLibrariesContext libraryContext)
        {
            this.libraryContext = libraryContext;
        }
        public IExportService<Library> GetExportService(string
       contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new LibraryExportService(libraryContext);
            }
            throw new NotImplementedException($"No export service implemented for libraries with content type {contentType}");
        }
        public IImportService<Library> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new LibraryImportService(libraryContext);
            }
            throw new NotImplementedException($"No import service implemented for libraries with content type {contentType}");
        }
    }
}