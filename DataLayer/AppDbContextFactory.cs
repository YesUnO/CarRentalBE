using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace DataLayer
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();



            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            dbContextOptionsBuilder
                .UseNpgsql(connectionString);

            return new ApplicationDbContext(dbContextOptionsBuilder.Options);
        }
    }
}
