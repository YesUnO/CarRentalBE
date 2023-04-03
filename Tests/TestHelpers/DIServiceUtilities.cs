using Core.Domain.User;
using Core.Infrastructure.Files;
using Core.Infrastructure.Options;
using DataLayer;
using DataLayer.Entities.Cars;
using DataLayer.Entities.Orders;
using DataLayer.Entities.User;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace TestHelpers;

public static class DIServiceUtilities
{
    public static IServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        return services;
    }

    public static IServiceCollection AddInMemoryDbContext(this IServiceCollection services)
    {

        services.AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase(databaseName: "yo"));
        services.Configure<OperationalStoreOptions>(x => { });
        return services;
    }

    public static IServiceCollection AddTestDbContext(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql("User ID =postgres;Password=nevermind;Server=localhost;Port=5432;Database=yoDb;Integrated Security=true;Pooling=true;Include Error Detail=true"));
        services.Configure<OperationalStoreOptions>(x => { });

        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();

        return services;
    }

    public static IServiceCollection AddUserServiceMockWithSignedInUser(this IServiceCollection services, ApplicationUser? desiredUser = null)
    {
        var user = desiredUser is null ? new ApplicationUser() : desiredUser;
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

    public static int AddOrderToExistingDbContext(this IServiceCollection services)
    {

        var serviceProvider = services.BuildServiceProvider();

        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        if (context is not null)
        {
            var customer = context.ApplicationUsers.FirstOrDefault();
            var car = new Car
            {
                Name = "yo",
            };
            if (customer is null)
            {
                return 0;
            }
            var order = new Order
            {
                Car = car,
                Customer = customer
            };


            context.Add(car);
            context.Add(order);
            context.SaveChanges();
            return order.Id;
        }
        return 0;
    }

    public static IServiceCollection CreateServiceCollectionForFileService(ApplicationUser? desiredUser = null)
    {
        var services = CreateServiceCollection();
        services.AddInMemoryDbContext();
        services.AddUserServiceMockWithSignedInUser(desiredUser);
        services.AddSingleton<FileService>();
        var options = Options.Create(new FileSettings
        {
            CloudmersiveApiKey = "8c6027b2-7bef-487d-977f-f22fb2e14579",
            Root = "C:\\vilem\\work\\test\\StoragetTest"
        });
        services.AddSingleton(options);
        return services;
    }

    public static FileService GetFileService(ApplicationUser? desiredUser = null)
    {
        var services = CreateServiceCollectionForFileService();
        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<FileService>();
    }
}