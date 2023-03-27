using DataLayer;
using DataLayer.Entities.Files;
using DTO;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Files
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private string _filePath = Environment.CurrentDirectory;

        public FileService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }


        public async Task SaveFileAsync(IFormFile file, string carName, FileType fileType)
        {
            var result = await SaveFileToDiskAsync(file, carName, fileType);

            if (result)
            {
                await SaveFiletoDb();
            }
        }

        private async Task<PDF> SaveFiletoDb()
        {
            var pdf = new PDF { RelativePath = _filePath };
            var savedPdf = _applicationDbContext.PDFs.Add(pdf);
            await _applicationDbContext.SaveChangesAsync();
            return savedPdf.Entity;
        }

        private string GetFileName(FileType fileType) => fileType switch
        {
            FileType.CarBackImage => "",
            FileType.CarPromoImage => "",
            FileType.CarFrontImage => "",
            FileType.CarSideImage => "",
            FileType.CarOtherSideImage => "",
            FileType.CarTrunkImage => "",
            FileType.CarDashboardImage => "",
            FileType.CarCabineImage => "",
            FileType.STKFrontImage => "",
            FileType.STKBackImage => "",
            FileType.TechnicLicenseFrontImage => "",
            FileType.TechnicLicenseBackImage => "",
            FileType.DriverseLicenseFrontImage => "",
            FileType.DriverseLicenseBackImage => "",
            FileType.CarAccidentImage => "",
            FileType.IdentificationCardFrontImage => "",
            FileType.IdentificationCardBackImage => "",
            FileType.InsurancePdf => "",
            FileType.CarPurchasePdf => "",
            _ => ""
        };

        private string GetFolderPath(string carName, FileType fileType)
        {
            return _filePath;
        }
        private async Task<bool> SaveFileToDiskAsync(IFormFile file, string carName, FileType fileType)
        {
            var path = GetFolderPath(carName, fileType);
            var name = GetFileName(fileType);
            using (var fileStream = new FileStream(Path.Combine(path, name), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return true;
        }

        private void CheckFileForAnitVirus(IFormFile file)
        {
            var scanner = new AntiVirus.Scanner();
        }
    }
}
