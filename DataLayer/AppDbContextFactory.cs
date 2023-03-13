﻿using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace DataLayer
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();




            //var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            //var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //dbContextOptionsBuilder
            //    .UseNpgsql(connectionString);
            //return new ApplicationDbContext(dbContextOptionsBuilder.Options, new OperationalStoreOptions());


            //var connectionString = "User ID =postgres;Password=nevermind;Server=localhost;Port=5432;Database=testDb;Integrated Security=true;Pooling=true;";
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            IServiceCollection services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
            services.Configure<OperationalStoreOptions>(x => { });

            var context = services.BuildServiceProvider().GetService<ApplicationDbContext>();
            return context;
        }
    }
}
