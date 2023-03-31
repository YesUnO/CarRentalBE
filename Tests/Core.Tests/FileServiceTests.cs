using Core.Infrastructure.Files;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers;

namespace Core.Tests
{
    public class FileServiceTests
    {
        //private methods, if testing need to be made public
        [Fact]
        public void SaveUserDocumentImageToDb()
        {
            //Arrange
            var services = DIServiceUtilities.CreateServiceCollection();
            services.AddInMemoryDbContext();
            services.AddUserServiceMockWithSignedInUser();
            services.AddSingleton<FileService>();

            var provider = services.BuildServiceProvider();

            var fileService = provider.GetService<FileService>();

            var context = provider.GetService<ApplicationDbContext>();

            //Act

            //fileService.SaveUserDocumentImageToDb(DTO.FileType.DriverseLicenseBackImage, "nejaka/cesta");

            //Assert
            var user = context.ApplicationUsers.Include(x=>x.DriversLicense).FirstOrDefault();
            Assert.NotNull(user);
            Assert.NotNull(user.DriversLicense);
            Assert.NotNull(user.DriversLicense.BackSideImage);
        }
    }
}