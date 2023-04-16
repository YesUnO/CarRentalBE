using DataLayer;
using DataLayer.Entities.Files;
using DTO;
using Microsoft.AspNetCore.Http;
using Cloudmersive.APIClient.NETCore.VirusScan.Client;
using Microsoft.Extensions.Options;
using Cloudmersive.APIClient.NETCore.VirusScan.Api;
using Microsoft.Extensions.Logging;
using Cloudmersive.APIClient.NETCore.VirusScan.Model;
using Microsoft.EntityFrameworkCore;
using Core.Domain.User;
using DataLayer.Entities.User;
using DataLayer.Entities.Orders;
using Core.Infrastructure.Files.Options;

namespace Core.Infrastructure.Files;

public class FileService : IFileService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly FileSettings _fileSettingsOptions;
    private readonly ILogger<FileService> _logger;
    private readonly IUserService _userService;
    private ApplicationUser? _applicationUser;
    private Order? _order;

    public FileService(ApplicationDbContext applicationDbContext,
                       IOptions<FileSettings> fileSettingsOptions,
                       ILogger<FileService> logger,
                       IUserService userService)
    {
        _applicationDbContext = applicationDbContext;
        _fileSettingsOptions = fileSettingsOptions.Value;
        _logger = logger;
        _userService = userService;
    }


    public async Task SaveCarReturningPhotoAsync(IFormFile file, int orderId, CarReturningImageType carReturningImageType)
    {
        var folderPath = GetReturningPhotoFolderPath(orderId);
        var fileName = carReturningImageType.ToString() + ".jpg";
        var filePath = await SaveFileToDiskAsync(file, folderPath, fileName);

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        await SaveOrderImageToDb(carReturningImageType, filePath);

    }
    
    public async Task SaveUserDocumentPhotoAsync(IFormFile file, UserDocumentImageType imageType)
    {
        var folderPath = GetUserDocumentFolderPath(imageType);
        var fileName = imageType.ToString() + ".jpg";
        var filePath = await SaveFileToDiskAsync(file, folderPath, fileName);

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }


        await SaveUserDocumentImageToDb(imageType, filePath);
    }
    public async Task<FileStream> GetUserDocumentPhoto(string mail, UserDocumentImageType userDocumentImageType)
    {
        var user = await _userService.GetUserByMail(mail);
        Image entity;
        switch (userDocumentImageType)
        {
            case UserDocumentImageType.DriverseLicenseFrontImage:
                entity = await _applicationDbContext.Images.FirstOrDefaultAsync(x=>x.Id == user.DriversLicense.FrontSideImage.Id);
                break;
            case UserDocumentImageType.DriverseLicenseBackImage:
                entity = await _applicationDbContext.Images.FirstOrDefaultAsync(x=>x.Id == user.DriversLicense.BackSideImage.Id);
                break;
            case UserDocumentImageType.IdentificationCardFrontImage:
                entity = await _applicationDbContext.Images.FirstOrDefaultAsync(x=>x.Id == user.IdentificationCard.FrontSideImage.Id);
                break;
            case UserDocumentImageType.IdentificationCardBackImage:
                entity = await _applicationDbContext.Images.FirstOrDefaultAsync(x=>x.Id == user.IdentificationCard.BackSideImage.Id);
                break;
            default: return null;
        }
        if (entity is null)
        {
            return null;
        }

        return new FileStream(entity.RelativePath, FileMode.Open);
    }

    #region private
    private async Task<string> SaveFileToDiskAsync(IFormFile file, string folderPath, string fileName)
    {
        var filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            return string.Empty;
        }

        //VirusScanResult? scanResult = null;

        //using (var memoryStream = new MemoryStream())
        //{
        //    await file.CopyToAsync(memoryStream);
        //    memoryStream.Seek(0, SeekOrigin.Begin);
        //    scanResult = CheckFileForVirus(memoryStream);
        //}

        //if (scanResult is null || !scanResult.CleanResult.HasValue || !scanResult.CleanResult.Value)
        //{
        //    return string.Empty;
        //}

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        return filePath;
    }

    private OrderImage GetOrderImageDbEntitity(CarReturningImageType carReturningImageType, string filePath) => carReturningImageType switch
    {
        CarReturningImageType.CarFrontImage => new OrderImage { CarImageType = CarImageType.CarFront, Order = _order, RelativePath = filePath },
        CarReturningImageType.CarBackImage => new OrderImage { CarImageType = CarImageType.CarBack, Order = _order, RelativePath = filePath },
        CarReturningImageType.CarSideImage => new OrderImage { CarImageType = CarImageType.CarSide, Order = _order, RelativePath = filePath },
        CarReturningImageType.CarOtherSideImage => new OrderImage { CarImageType = CarImageType.CarOtherSide, Order = _order, RelativePath = filePath },
        CarReturningImageType.CarTrunkImage => new OrderImage { CarImageType = CarImageType.CarTrunk, Order = _order, RelativePath = filePath },
        CarReturningImageType.CarDashboardImage => new OrderImage { CarImageType = CarImageType.CarDashboard, Order = _order, RelativePath = filePath },
        CarReturningImageType.CarCabineImage => new OrderImage { CarImageType = CarImageType.CarCabine, Order = _order, RelativePath = filePath },
        _ => throw new NotImplementedException(),
    };

    private async Task<bool> SaveUserDocumentImageToDb(UserDocumentImageType userDocumentImageType, string filePath)
    {
        var dbEntity = new UserDocumentImage { RelativePath = filePath };
        var user = GetAndSetIfNullApplicationUser();

        switch (userDocumentImageType)
        {
            case UserDocumentImageType.DriverseLicenseFrontImage:
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

            case UserDocumentImageType.DriverseLicenseBackImage:
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
            case UserDocumentImageType.IdentificationCardFrontImage:
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
            case UserDocumentImageType.IdentificationCardBackImage:
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

        _applicationDbContext.Add(dbEntity);
        _applicationDbContext.Update(user);
        await _applicationDbContext.SaveChangesAsync();
        return true;
    }

    private async Task<bool> SaveOrderImageToDb(CarReturningImageType carReturningImageType, string filePath)
    {
        var dbEntity = GetOrderImageDbEntitity(carReturningImageType, filePath);

        _applicationDbContext.Add(dbEntity);
        _applicationDbContext.SaveChanges();
        return true;

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

        var path = Path.Combine(_fileSettingsOptions.Root, "Cars", carName, year, month, "Orders", order.Id.ToString());
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    private string GetUserDocumentFolderPath(UserDocumentImageType userDocumentImageType)
    {
        var signedInUser = GetAndSetIfNullApplicationUser();
        string? documentType;
        if (userDocumentImageType is UserDocumentImageType.DriverseLicenseBackImage or UserDocumentImageType.DriverseLicenseFrontImage)
        {
            documentType = "DriverseLicense";
        }

        else if (userDocumentImageType is UserDocumentImageType.IdentificationCardBackImage or UserDocumentImageType.IdentificationCardFrontImage)
        {
            documentType = "IdentificationCard";
        }
        else
        {
            return string.Empty;
        }

        var path = Path.Combine(_fileSettingsOptions.Root, "Users", signedInUser.Id.ToString(), documentType);
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
    #endregion
}
