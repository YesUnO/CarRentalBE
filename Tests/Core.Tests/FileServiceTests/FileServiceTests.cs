using Cloudmersive.APIClient.NETCore.VirusScan.Model;
using Core.Infrastructure.Files;
using DataLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers;

namespace Core.Tests.FileServiceTests;

public class FileServiceTests
{
    //private methods, if testing need to be made public
    [Fact]
    public void SaveUserDocumentImageToDb()
    {
        //Arrange
        var services = DIServiceUtilities.CreateServiceCollectionForFileService();

        var provider = services.BuildServiceProvider();
        var fileService = provider.GetService<FileService>();

        //Act
        //fileService.SaveUserDocumentImageToDb(DTO.FileType.DriverseLicenseBackImage, "nejaka/cesta");

        //Assert
        var context = provider.GetService<ApplicationDbContext>();
        var user = context!.ApplicationUsers.Include(x => x.DriversLicense).FirstOrDefault();
        Assert.NotNull(user);
        Assert.NotNull(user.DriversLicense);
        Assert.NotNull(user.DriversLicense.BackSideImage);
    }

    [Fact]
    public void CheckFileForVirus()
    {
        //Arrange
        var fileService = DIServiceUtilities.GetFileService();

        //Act
        VirusScanResult? result = null;
        using var memoryStream = new MemoryStream();

        using (var fileStream = new FileStream("C:\\vilem\\work\\test\\yo.jpg", FileMode.Open))
        {
            fileStream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            //result = fileService.CheckFileForVirus(memoryStream);
        }

        //Assert
        Assert.NotNull(result);
        Assert.True(result.CleanResult);
    }

    [Fact]
    public async Task SaveFileToDiskAsync()
    {
        //Arrange
        var services = DIServiceUtilities.CreateServiceCollectionForFileService();
        var orderId = DIServiceUtilities.AddOrderToExistingDbContext(services);
        var provider = services.BuildServiceProvider();
        var fileService = provider.GetService<FileService>();
        var context = provider.GetService<ApplicationDbContext>();


        //Act
        using (var filesStream = new FileStream("C:\\vilem\\work\\test\\yo.jpg", FileMode.Open))
        {
            IFormFile file = new FormFile(filesStream, 0, filesStream.Length, null, "yo");
            await fileService.SaveCarReturningPhotoAsync(file, orderId, DTO.CarReturningImageType.CarBackImage);
        }


        //Assert
        var order = context.Orders.Include(x => x.ReturningPhotos).FirstOrDefault();
        Assert.NotNull(order);
        Assert.True(order.ReturningPhotos.Any());
    }
}