using Core.Domain.User;
using Core.Infrastructure.Options;
using DataLayer;
using DataLayer.Entities.User;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestHelpers
{
    public static class DIServiceUtilities
    {
        public static IServiceCollection AddInMemoryDbContext(this IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase(databaseName: "yo"));
            services.Configure<OperationalStoreOptions>(x => { });
            return services;
        }

        public static IServiceCollection CreateServiceCollection()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            return services;
        }

        public static IServiceCollection AddUserServiceMockWithSignedInUser(this IServiceCollection services, ApplicationUser? desiredUser = null)
        {
            var user = desiredUser is null?  new ApplicationUser() : desiredUser;

            var userServicceMock = new Mock<IUserService>();

            userServicceMock.Setup(x => x.GetSignedInUser()).Returns(user);

            services.AddSingleton(userServicceMock.Object);

            var serviceProvider = services.BuildServiceProvider();

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (context is not null)
            {
                context.ApplicationUsers.Add(user);
                context.SaveChanges();
            }
            
            return services;
        }
    }
}