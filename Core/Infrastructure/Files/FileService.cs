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
using Microsoft.EntityFrameworkCore;
using Core.Domain.User;
using DataLayer.Entities.User;
using DataLayer.Entities.Orders;

namespace Core.Infrastructure.Files
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly FileSettings _fileSettingsOptions;
        private string _filePath = Environment.CurrentDirectory;
        private readonly ILogger<FileService> _logger;
        private readonly IUserService _userService;
        private ApplicationUser _applicationUser;
        private Order _order;

        public FileService(ApplicationDbContext applicationDbContext, IOptions<FileSettings> fileSettingsOptions, ILogger<FileService> logger, IUserService userService)
        {
            _applicationDbContext = applicationDbContext;
            _fileSettingsOptions = fileSettingsOptions.Value;
            _logger = logger;
            _userService = userService;
        }


        public async Task SaveFileAsync(IFormFile file, FileType fileType, int? orderId, int? userDocumentId)
        {
            var filePath = await SaveFileToDiskAsync(file, fileType, userDocumentId);

            if (!string.IsNullOrEmpty(filePath))
            {
                await SaveFileToDb(fileType, filePath);
            }
        }
        private async Task<string> SaveFileToDiskAsync(IFormFile file, FileType fileType, int? orderId)
        {
            var path = GetFolderPath(fileType, orderId);
            var name = GetFileName(fileType);
            var filePath = Path.Combine(path, name);

            if (File.Exists(filePath))
            {
                return string.Empty;
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var scanResult = CheckFileForAnitVirus(fileStream);
                if (scanResult is not null && scanResult.CleanResult.HasValue && scanResult.CleanResult.Value)
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            return filePath;
        }

       

        private DbEntitity GetDbEntitity<DbEntitity>(FileType fileType, string filePath) where DbEntitity : Image => fileType switch
        {
            FileType.CarFrontImage => new OrderImage { CarImageType = CarImageType.CarFront, Order = _order, RelativePath = filePath },
            FileType.CarBackImage => throw new NotImplementedException(),
            FileType.CarSideImage => throw new NotImplementedException(),
            FileType.CarOtherSideImage => throw new NotImplementedException(),
            FileType.CarTrunkImage => throw new NotImplementedException(),
            FileType.CarDashboardImage => throw new NotImplementedException(),
            FileType.CarCabineImage => throw new NotImplementedException(),
            FileType.STKFrontImage => throw new NotImplementedException(),
            FileType.STKBackImage => throw new NotImplementedException(),
            FileType.TechnicLicenseFrontImage => throw new NotImplementedException(),
            FileType.TechnicLicenseBackImage => throw new NotImplementedException(),
            FileType.DriverseLicenseFrontImage => throw new NotImplementedException(),
            FileType.DriverseLicenseBackImage => throw new NotImplementedException(),
            FileType.IdentificationCardFrontImage => throw new NotImplementedException(),
            FileType.IdentificationCardBackImage => throw new NotImplementedException(),
            FileType.CarAccidentImage => throw new NotImplementedException(),
            FileType.InsurancePdf => throw new NotImplementedException(),
            FileType.CarPurchasePdf => throw new NotImplementedException(),
            FileType.CarPromoImage => throw new NotImplementedException(),
        };

        private async Task<bool> SaveFileToDb(FileType fileType, string filePath)
        {
            switch (fileType)
            {
                case FileType.CarFrontImage:
                case FileType.CarBackImage:
                case FileType.CarSideImage:
                case FileType.CarOtherSideImage:
                case FileType.CarTrunkImage:
                case FileType.CarDashboardImage:
                case FileType.CarCabineImage:
                    return false;
                case FileType.DriverseLicenseFrontImage:
                case FileType.DriverseLicenseBackImage:
                case FileType.IdentificationCardFrontImage:
                case FileType.IdentificationCardBackImage:
                    return false;
                default:
                    return false;
            }
        }


        private string GetFileName(FileType fileType) => fileType switch
        {
            FileType.CarBackImage => FileType.CarBackImage.ToString(),
            FileType.CarPromoImage => FileType.CarPromoImage.ToString(),
            FileType.CarFrontImage => FileType.CarFrontImage.ToString(),
            FileType.CarSideImage => FileType.CarSideImage.ToString(),
            FileType.CarOtherSideImage => FileType.CarOtherSideImage.ToString(),
            FileType.CarTrunkImage => FileType.CarTrunkImage.ToString(),
            FileType.CarDashboardImage => FileType.CarDashboardImage.ToString(),
            FileType.CarCabineImage => FileType.CarCabineImage.ToString(),
            FileType.STKFrontImage => FileType.STKFrontImage.ToString(),
            FileType.STKBackImage => FileType.STKBackImage.ToString(),
            FileType.TechnicLicenseFrontImage => FileType.TechnicLicenseFrontImage.ToString(),
            FileType.TechnicLicenseBackImage => FileType.TechnicLicenseBackImage.ToString(),
            FileType.DriverseLicenseFrontImage => FileType.DriverseLicenseFrontImage.ToString(),
            FileType.DriverseLicenseBackImage => FileType.DriverseLicenseBackImage.ToString(),
            FileType.IdentificationCardFrontImage => FileType.IdentificationCardFrontImage.ToString(),
            FileType.IdentificationCardBackImage => FileType.IdentificationCardBackImage.ToString(),
            FileType.CarAccidentImage => FileType.CarAccidentImage.ToString(),
            FileType.InsurancePdf => FileType.InsurancePdf.ToString(),
            FileType.CarPurchasePdf => FileType.CarPurchasePdf.ToString(),
            _ => ""
        };

        private string GetFolderPath(FileType fileType, int? orderId)
        {
            var path = orderId == null ? GetUserDocumentFolderPath(fileType): GetReturningPhotoFolderPath((int)orderId);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private ApplicationUser GetAndSetIfNullApplicationUser()
        {
            if (_applicationUser is null)
            {
                _applicationUser = _userService.GetSignedInUser();
            }
            return _applicationUser;
        }

        private Order GetAndSetIfNullOrder(int orderID)
        {
            if (_order is null)
            {
                _order = _applicationDbContext.Orders.Include(x => x.Car).FirstOrDefault(x => x.Id == orderID);
            }

            return _order;
        }

        private string GetReturningPhotoFolderPath(int orderId) 
        {
            var order = GetAndSetIfNullOrder(orderId);
            if (order is null)
            {
                return string.Empty;
            }

            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day.ToString();
            var carName = order.Car.Name;

            return Path.Combine(_fileSettingsOptions.Root, "Cars", carName, year, month, day, order.Id.ToString());
        }

        private string GetUserDocumentFolderPath(FileType fileType)
        {
            var signedInUser = GetAndSetIfNullApplicationUser();
            var documentType = string.Empty;

            if (fileType is FileType.DriverseLicenseBackImage or FileType.DriverseLicenseFrontImage)
            {
                documentType = "DriverseLicense";
            }

            else if (fileType is FileType.IdentificationCardBackImage or FileType.IdentificationCardFrontImage)
            {
                documentType = "IdentificationCard";
            }
            else
            {
                return string.Empty;
            }

            return Path.Combine(_fileSettingsOptions.Root, "Users", signedInUser.Id.ToString(), documentType);
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
