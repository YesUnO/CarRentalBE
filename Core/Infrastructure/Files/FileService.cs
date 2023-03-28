using DataLayer;
using DataLayer.Entities.Files;
using DTO;
using Microsoft.AspNetCore.Http;
using Cloudmersive.APIClient.NETCore.VirusScan.Client;
using Core.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Cloudmersive.APIClient.NETCore.VirusScan.Api;
using Microsoft.Extensions.Logging;
using Cloudmersive.APIClient.NETCore.VirusScan.Model;
using Microsoft.AspNetCore.Identity;

namespace Core.Infrastructure.Files
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly FileSettings _fileSettingsOptions;
        private string _filePath = Environment.CurrentDirectory;
        private readonly ILogger<FileService> _logger;

        public FileService(ApplicationDbContext applicationDbContext, IOptions<FileSettings> fileSettingsOptions, ILogger<FileService> logger)
        {
            _applicationDbContext = applicationDbContext;
            _fileSettingsOptions = fileSettingsOptions.Value;
            _logger = logger;
        }


        public async Task SaveFileAsync(IFormFile file, FileType fileType, int? orderId, int? userDocumentId)
        {
            var result = await SaveFileToDiskAsync(file, fileType);

            if (result)
            {
                await SaveFiletoDb();
            }
        }
        private async Task<bool> SaveFileToDiskAsync(IFormFile file, FileType fileType)
        {
            var path = GetFolderPath(fileType);
            var name = GetFileName(fileType);
            using (var fileStream = new FileStream(Path.Combine(path, name), FileMode.Create))
            {
                var scanResult = CheckFileForAnitVirus(fileStream);
                if (scanResult.CleanResult.HasValue && scanResult.CleanResult.Value)
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            return true;
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
            FileType.IdentificationCardFrontImage => "",
            FileType.IdentificationCardBackImage => "",
            FileType.CarAccidentImage => "",
            FileType.InsurancePdf => "",
            FileType.CarPurchasePdf => "",
            _ => ""
        };

        private string GetFolderPath(FileType fileType)
        {
            //var signedInUser = ;

            

            return _filePath;
        }

        private VirusScanResult CheckFileForAnitVirus(FileStream fileStream)
        {
            
            try
            {
                Configuration.Default.AddApiKey("Apikey", _fileSettingsOptions.CloudmersiveApiKey);

                var apiInstance = new ScanApi(); 
                var scanResult = apiInstance.ScanFile(fileStream);
                return scanResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Cloudmersive scan for virus failed");
                return null;
            }

            //var path = file.Get
            //var scanner = new AntiVirus.Scanner();
        }
    }
}
