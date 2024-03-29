﻿using DataLayer;
using DataLayer.Entities.Files;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Core.Domain.User;
using DataLayer.Entities.User;
using DataLayer.Entities.Orders;
using Core.Infrastructure.Files.Options;
using Azure.Storage;
using Azure.Storage.Blobs;
using Core.Infrastructure.Helpers;
using Google.Cloud.Storage.V1;

namespace Core.Infrastructure.Files;

public class FileService : IFileService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly AzureStorageConfig _azureStorageConfig;
    private readonly ILogger<FileService> _logger;
    private readonly IUsersService _userService;
    private ApplicationUser? _applicationUser;
    private Order? _order;
    private readonly StorageClient _googleStorageClient;

    public FileService(ApplicationDbContext applicationDbContext,
                       IOptions<AzureStorageConfig> azureStorageConfig,
                       ILogger<FileService> logger,
                       IUsersService userService,
                       StorageClient googleStorageClient)
    {
        _applicationDbContext = applicationDbContext;
        _azureStorageConfig = azureStorageConfig.Value;
        _logger = logger;
        _userService = userService;
        _googleStorageClient = googleStorageClient;
    }


    public async Task SaveCarReturningPhotoAsync(IFormFile file, int orderId, CarReturningImageType carReturningImageType, string loggedinUserMail)
    {
        var fileName = GetReturningPhotoFileName(orderId, carReturningImageType);
        var fileUrl = "";

        using (Stream stream = file.OpenReadStream())
        {
            fileUrl = await UploadFileToStorage(stream, fileName, false);
        }

        if (string.IsNullOrEmpty(fileUrl))
        {
            throw new Exception("Saving image to blob failed");
        }

        await SaveOrderImageToDb(carReturningImageType, fileUrl);
    }

    public async Task SaveUserDocumentPhotoAsync(IFormFile file, UserDocumentImageType imageType, string loggedinUserMail)
    {
        var fileName = GetUserDocumentFileName(imageType, loggedinUserMail);
        var fileUrl = "";

        using (Stream stream = file.OpenReadStream())
        {
            fileUrl = await UploadFileToStorage(stream, fileName, true);
        }


        if (string.IsNullOrEmpty(fileUrl))
        {
            throw new Exception("Saving image to blob failed");
        }

        await SaveUserDocumentImageToDb(imageType, fileUrl, loggedinUserMail);
    }

    public async Task SaveCarProfilePickAsync(int carId, IFormFile file)
    {
        var fileName = GetCarProfilePicFileName(carId);
        var fileUrl = "";

        using (Stream stream = file.OpenReadStream())
        {
            fileUrl = await UploadFileToStorage(stream, fileName, false);
        }


        if (string.IsNullOrEmpty(fileUrl))
        {
            throw new Exception("Saving image to blob failed");
        }

        await SaveCarPickImageToDb(carId, fileUrl);
    }

    public async Task<FileStream> GetUserDocumentPhoto(string mail, UserDocumentImageType userDocumentImageType)
    {
        var user = await _userService.GetUserByMailAsync(mail, true);
        Image entity;
        switch (userDocumentImageType)
        {
            case UserDocumentImageType.DriverseLicenseFrontImage:
                entity = await _applicationDbContext.Images.FirstOrDefaultAsync(x => x.Id == user.DriversLicense.FrontSideImage.Id);
                break;
            case UserDocumentImageType.DriverseLicenseBackImage:
                entity = await _applicationDbContext.Images.FirstOrDefaultAsync(x => x.Id == user.DriversLicense.BackSideImage.Id);
                break;
            case UserDocumentImageType.IdentificationCardFrontImage:
                entity = await _applicationDbContext.Images.FirstOrDefaultAsync(x => x.Id == user.IdentificationCard.FrontSideImage.Id);
                break;
            case UserDocumentImageType.IdentificationCardBackImage:
                entity = await _applicationDbContext.Images.FirstOrDefaultAsync(x => x.Id == user.IdentificationCard.BackSideImage.Id);
                break;
            default: return null;
        }
        if (entity is null)
        {
            return null;
        }

        return new FileStream(entity.RelativePath, FileMode.Open);
    }


    public async Task DeleteCarProfilePickAsync(int carId)
    {
        var car = _applicationDbContext.Cars.Include(x => x.ProfilePic).FirstOrDefault(x => x.Id == carId);
        if (car == null)
        {
            throw new Exception("Unexisting car");
        }

        var picture = car.ProfilePic;
        if (picture != null)
        {
            car.ProfilePic = null;
            _applicationDbContext.Remove(picture);
            await _applicationDbContext.SaveChangesAsync();
        }
    }


    #region private

    private async Task<string> UploadFileToStorage(Stream fileStream, string fileName, bool secret)
    {
        try
        {
            return await UploadFileToGoogleStorage(fileStream, fileName, secret);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Google storage doesnt work");
            throw;
        }
        
    }

    private async Task<string> UploadFileToAzureStorageAsync(Stream fileStream, string fileName, bool secret)
    {
        string container = secret ? _azureStorageConfig.DocumentImageContainer : _azureStorageConfig.ImageContainer;

        Uri blobUri = new Uri("https://" +
                                  _azureStorageConfig.AccountName +
                                  ".blob.core.windows.net/" +
                                  container +
                                  "/" + fileName);

        StorageSharedKeyCredential storageCredentials =
                new StorageSharedKeyCredential(_azureStorageConfig.AccountName, _azureStorageConfig.AccountKey);

        BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

        // Upload the file
        await blobClient.UploadAsync(fileStream);

        return await Task.FromResult(blobUri.ToString());
    }

    private async Task<string> UploadFileToGoogleStorage(Stream fileStream, string fileName, bool secret)
    {
        var bucketName = "car_rental_img";
        var folderName = secret ? "private" : "public";
        var objName = folderName+ "/" + fileName;
        var storageObj = await _googleStorageClient.UploadObjectAsync(bucketName, objName, null, fileStream);
        var url = $"https://storage.googleapis.com/{bucketName}/{objName}";
        return url;
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

    private async Task<bool> SaveUserDocumentImageToDb(UserDocumentImageType userDocumentImageType, string filePath, string loggedinUsermail)
    {
        var dbEntity = new UserDocumentImage { RelativePath = filePath };
        var user = GetAndSetIfNullApplicationUser(loggedinUsermail);

        switch (userDocumentImageType)
        {
            case UserDocumentImageType.DriverseLicenseFrontImage:
                if (user.DriversLicense == null)
                {
                    user.DriversLicense = new UserDocument
                    {
                        FrontSideImage = dbEntity,
                        BackSideImage = new UserDocumentImage { RelativePath = "empty" }
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
                        FrontSideImage = new UserDocumentImage { RelativePath = "empty" }
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
                        BackSideImage = new UserDocumentImage { RelativePath = "empty" }
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
                    user.IdentificationCard = new UserDocument 
                    { 
                        BackSideImage = dbEntity,
                        FrontSideImage = new UserDocumentImage { RelativePath = "empty" }
                    };
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

    private async Task SaveCarPickImageToDb(int carId, string filePath)
    {
        var car = await _applicationDbContext.Cars.FirstOrDefaultAsync(x=>x.Id == carId);
        if (car is null)
        {
            throw new Exception("couldnt find car in db");
        }
        car.ProfilePic = new Image { RelativePath = filePath };
        await _applicationDbContext.SaveChangesAsync();
    }

    private async Task<bool> SaveOrderImageToDb(CarReturningImageType carReturningImageType, string filePath)
    {
        var dbEntity = GetOrderImageDbEntitity(carReturningImageType, filePath);

        _applicationDbContext.Add(dbEntity);
        _applicationDbContext.SaveChanges();
        return true;

    }

    private string GetReturningPhotoFileName(int orderId, CarReturningImageType carReturningImageType)
    {
        var order = GetAndSetIfNullOrder(orderId);
        var id = FileHelper.GetDateShortId();
        return $"{carReturningImageType}{id}{orderId}.jpg";
    }

    private string GetUserDocumentFileName(UserDocumentImageType userDocumentImageType, string loggedinUserMail)
    {
        var signedInUser = GetAndSetIfNullApplicationUser(loggedinUserMail);
        var id = FileHelper.GetDateShortId();
        return $"{userDocumentImageType.ToString()}{id}{signedInUser.Email}.jpg";
    }

    private string GetCarProfilePicFileName(int carId)
    {
        var id = FileHelper.GetDateShortId();
        return $"{carId}carPic{id}.jpg";
    }

    private ApplicationUser GetAndSetIfNullApplicationUser(string mail)
    {
        if (_applicationUser is null)
        {
            _applicationUser = _userService.GetUserByMailAsync(mail, true).Result;
        }
        return _applicationUser;
    }

    private Order GetAndSetIfNullOrder(int orderID)
    {
        if (_order is null)
        {
            _order = _applicationDbContext.Orders.FirstOrDefault(x => x.Id == orderID);
        }

        return _order;
    }

    #endregion
}
