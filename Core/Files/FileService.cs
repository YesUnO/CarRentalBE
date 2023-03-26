using DataLayer;
using DataLayer.Entities.Files;
using DTO;
using Microsoft.AspNetCore.Http;

namespace Core.Files
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private string _filePath = Environment.CurrentDirectory;

        public FileService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<bool> SaveFileToDiskAsync(IFormFile file, string carName, FileType fileType)
        {
            var path = GetFolderPath(carName, fileType);
            return true;
        }



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

        private string GetFolderPath(string carName, FileType fileType)
        {
            return _filePath;
        }
    }
}
