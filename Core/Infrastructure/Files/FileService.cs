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
        private readonly ILogger<FileService> _logger;
        private readonly IUserService _userService;
        private ApplicationUser? _applicationUser;
        private Order? _order;

        public FileService(ApplicationDbContext applicationDbContext, IOptions<FileSettings> fileSettingsOptions, ILogger<FileService> logger, IUserService userService)
        {
            _applicationDbContext = applicationDbContext;
            _fileSettingsOptions = fileSettingsOptions.Value;
            _logger = logger;
            _userService = userService;
        }


        public async Task SaveFileAsync(IFormFile file, FileType fileType, int? orderId, int? userDocumentId)
        {
            var filePath = await SaveFileToDiskAsync(file, fileType, orderId);

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            if (IsUserDocumentFileType(fileType))
            {
                await SaveUserDocumentImageToDb(fileType, filePath);
            }
            else
            {
                await SaveOrderImageToDb(fileType, filePath);
            }

        }
        private async Task<string> SaveFileToDiskAsync(IFormFile file, FileType fileType, int? orderId)
        {
            var path = GetAndEnsureCreatedFolderPath(fileType, orderId);
            var name = fileType.ToString();
            var filePath = Path.Combine(path, name);

            if (File.Exists(filePath))
            {
                return string.Empty;
            }

            VirusScanResult? scanResult = null;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                scanResult = CheckFileForVirus(memoryStream);
            }

            if (scanResult is null || !scanResult.CleanResult.HasValue || !scanResult.CleanResult.Value)
            {
                return string.Empty;
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return filePath;
        }

        private bool IsUserDocumentFileType(FileType fileType)
        {
            var userDocumentsFileTypes = new FileType[] {
                FileType.DriverseLicenseFrontImage ,
                FileType.DriverseLicenseBackImage ,
                FileType.IdentificationCardFrontImage ,
                FileType.IdentificationCardBackImage
            };

            return userDocumentsFileTypes.Contains(fileType);
        }

        private OrderImage GetOrderImageDbEntitity(FileType fileType, string filePath) => fileType switch
        {
            FileType.CarFrontImage => new OrderImage { CarImageType = CarImageType.CarFront, Order = _order, RelativePath = filePath },
            FileType.CarBackImage => new OrderImage { CarImageType = CarImageType.CarBack, Order = _order, RelativePath = filePath },
            FileType.CarSideImage => new OrderImage { CarImageType = CarImageType.CarSide, Order = _order, RelativePath = filePath },
            FileType.CarOtherSideImage => new OrderImage { CarImageType = CarImageType.CarOtherSide, Order = _order, RelativePath = filePath },
            FileType.CarTrunkImage => new OrderImage { CarImageType = CarImageType.CarTrunk, Order = _order, RelativePath = filePath },
            FileType.CarDashboardImage => new OrderImage { CarImageType = CarImageType.CarDashboard, Order = _order, RelativePath = filePath },
            FileType.CarCabineImage => new OrderImage { CarImageType = CarImageType.CarCabine, Order = _order, RelativePath = filePath },
            _ => throw new NotImplementedException(),
        };

        private async Task<bool> SaveUserDocumentImageToDb(FileType fileType, string filePath)
        {
            var dbEntity = new UserDocumentImage { RelativePath = filePath };
            var user = GetAndSetIfNullApplicationUser();

            switch (fileType)
            {
                case FileType.DriverseLicenseFrontImage:
                    if (user.DriversLicense == null)
                    {
                        user.DriversLicense = new UserDocument
                        {
                            FrontSideImage = dbEntity,
                        };
                    }
                    else
                    {
                        user.DriversLicense.FrontSideImage = dbEntity;
                    }
                    break;

                case FileType.DriverseLicenseBackImage:
                    if (user.DriversLicense == null)
                    {
                        user.DriversLicense = new UserDocument
                        {
                            BackSideImage = dbEntity,
                        };
                    }
                    else
                    {
                        user.DriversLicense.BackSideImage = dbEntity;
                    }
                    break;
                case FileType.IdentificationCardFrontImage:
                    if (user.IdentificationCard == null)
                    {
                        user.IdentificationCard = new UserDocument
                        {
                            FrontSideImage = dbEntity,
                        };
                    }
                    else
                    {
                        user.IdentificationCard.FrontSideImage = dbEntity;
                    }
                    break;
                case FileType.IdentificationCardBackImage:
                    if (user.IdentificationCard == null)
                    {
                        user.IdentificationCard = new UserDocument { BackSideImage = dbEntity };
                    }
                    else
                    {
                        user.IdentificationCard.BackSideImage = dbEntity;
                    }
                    break;
                default:
                    break;
            }

            _applicationDbContext.Update(user);
            _applicationDbContext.SaveChanges();
            return true;
        }

        private async Task<bool> SaveOrderImageToDb(FileType fileType, string filePath)
        {
            var dbEntity = GetOrderImageDbEntitity(fileType, filePath);

            _applicationDbContext.Add(dbEntity);
            _applicationDbContext.SaveChanges();
            return true;

        }

        private string GetAndEnsureCreatedFolderPath(FileType fileType, int? orderId)
        {
            var path = orderId == null ? GetUserDocumentFolderPath(fileType) : GetReturningPhotoFolderPath((int)orderId);

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

            return Path.Combine(_fileSettingsOptions.Root, "Cars", carName, year, month, "Orders", order.Id.ToString());
        }

        private string GetUserDocumentFolderPath(FileType fileType)
        {
            var signedInUser = GetAndSetIfNullApplicationUser();
            string? documentType;
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

        private VirusScanResult CheckFileForVirus(MemoryStream fileStream)
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
                _logger.LogError(ex, "Cloudmersive scan for virus failed");
                return null;
            }

            //var path = file.Get
            //var scanner = new AntiVirus.Scanner();
        }
    }
}
