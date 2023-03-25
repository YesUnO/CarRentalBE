using DataLayer;
using DataLayer.Entities.Files;

namespace Core.Files
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FileService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        private string _filePath = Environment.CurrentDirectory;
        public async Task<PDF> SaveFileAsync()
        {
            return await SavePDFtoDb();
        }

        private async Task<PDF> SavePDFtoDb()
        {
            var pdf = new PDF { RelativePath = _filePath };
            var savedPdf = _applicationDbContext.PDFs.Add(pdf);
            await _applicationDbContext.SaveChangesAsync();
            return savedPdf.Entity;
        }
    }
}
