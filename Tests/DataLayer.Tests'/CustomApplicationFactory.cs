
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace DataLayer.Tests_
{
    public class CustomApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    x => x.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (dbContextDescriptor is not null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var dbConnectionDescriptor = services.SingleOrDefault(
                    x => x.ServiceType == typeof(DbConnection));

                if (dbConnectionDescriptor is not null)
                {
                    services.Remove(dbConnectionDescriptor);
                }

                services.AddSingleton<DbConnection>(x =>
                {
                    var connection = new SqliteConnection("DataSource:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<ApplicationDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });

            });
            
            //builder.Use
        }
    }
}
