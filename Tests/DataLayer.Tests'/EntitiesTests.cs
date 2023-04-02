using DataLayer.Entities.Files;
using DataLayer.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestHelpers;

namespace DataLayer.Tests_
{
    public class EntitiesTests
    {
        [Fact]
        public void UserDocumentEntity_AddToApplicationUsesrEntityTest()
        {
            //Assert
            var services = DIServiceUtilities.CreateServiceCollection();
            services.AddTestDbContext();
            services.AddUserServiceMockWithSignedInUser();
            var provider = services.BuildServiceProvider();
            var context = provider.GetService<ApplicationDbContext>();
            var userDocument = new UserDocument();
            context.Add(userDocument);
            context.SaveChanges();

            var user = context.ApplicationUsers.FirstOrDefault();
            //Act

            user.DriversLicense = userDocument;
            
            context.SaveChanges();
            
            //Assert
            var test = context.ApplicationUsers.Include(x=>x.DriversLicense).FirstOrDefault();
            Assert.NotNull(test.DriversLicense);
        }

        [Fact]
        public void UserDocumentImageEntity_AddToUserDocumentEntityTest()
        {
            //Assert
            var services = DIServiceUtilities.CreateServiceCollection();
            services.AddInMemoryDbContext();
            services.AddUserServiceMockWithSignedInUser();
            var provider = services.BuildServiceProvider();
            var context = provider.GetService<ApplicationDbContext>();

            var userDocument = new UserDocument();
            var user = context.ApplicationUsers.FirstOrDefault();
            user.DriversLicense = userDocument;
            context.SaveChanges();
            //Act

            var backImage = new UserDocumentImage { RelativePath = "yo" };
            var testUser = context.ApplicationUsers.Include(x => x.DriversLicense).FirstOrDefault();
            testUser.DriversLicense.BackSideImage = backImage;
            context.SaveChanges();

            //Assert
            var test = context.UserDocuments.Include(x=>x.BackSideImage).FirstOrDefault();

            Assert.NotNull(test.BackSideImage);
        }
    }

}