using DataLayer.Entities.Cars;
using DataLayer.Entities.Files;
using DataLayer.Entities.Orders;
using DataLayer.Entities.User;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.EntityFramework.Extensions;
using Duende.IdentityServer.EntityFramework.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DataLayer
{
    public class ApplicationDbContext : IdentityDbContext, IPersistedGrantDbContext
    {
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options)
        {
            _operationalStoreOptions = operationalStoreOptions;
        }

        //Identity Setup
        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }
        public DbSet<Key> Keys { get; set; }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();


        //Application entities
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarDocument> CarDocuments { get; set; }
        public DbSet<CarInsurance> CarInsurances { get; set; }
        public DbSet<CarSpecification> CarSpecifications { get; set; }

        public DbSet<Image> Images { get; set; }
        public DbSet<PDF> PDFs { get; set; }

        public DbSet<Accident> Accidents { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<PaymentCard> PaymentCards { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Image>()
                .HasDiscriminator<string>("ImageType")
                .HasValue<Image>("Base")
                .HasValue<OrderImage>("Order")
                .HasValue<CarDocumentImage>("CarDocument")
                .HasValue<UserDocumentImage>("UserDocument")
                .HasValue<AccidentImage>("Accident");

            builder.Entity<CarDocument>()
                .HasOne(x => x.FrontSideImage)
                .WithMany(x=>x.FrontCarDocuments)
                .HasForeignKey("FrontSideImageId");

            builder.Entity<CarDocument>()
                .HasOne(x => x.BackSideImage)
                .WithMany(x => x.BackCarDocuments)
                .HasForeignKey("BackSideImageId");

            builder.Entity<UserDocument>()
                .HasOne(x => x.FrontSideImage)
                .WithMany(x=>x.FrontUserDocuments)
                .HasForeignKey("FrontSideImageId");

            builder.Entity<UserDocument>()
                .HasOne(x => x.BackSideImage)
                .WithMany(x=>x.BackUserDocuments)
                .HasForeignKey("BackSideImageId");

            builder.Entity<ApplicationUser>()
                .HasOne(x => x.IdentificationCard)
                .WithMany(x => x.IdentificationLicenseUsers);

            builder.Entity<ApplicationUser>()
                .HasOne(x => x.DriversLicense)
                .WithMany(x => x.DriverLicenseUsers);

            builder.Entity<ApplicationUser>()
                .HasMany(x => x.Orders)
                .WithOne(x => x.Customer);

            builder.Entity<Car>()
                .HasOne(x => x.TechnicLicense)
                .WithMany(x => x.TechnicLicenseUsers);

            builder.Entity<Car>()
                .HasOne(x => x.STK)
                .WithMany(x => x.STKUsers);

            builder.Entity<Car>()
                .HasMany(x => x.Orders)
                .WithOne(x => x.Car);

            builder.Entity<Car>()
                .HasMany(x => x.Accidents)
                .WithOne(x => x.Car);

            builder.Entity<Order>()
                .HasMany(x => x.ReturningPhotos)
                .WithOne(x => x.Order);

            builder.Entity<Order>()
                .HasMany(x => x.Payments)
                .WithOne(x => x.Order);

            builder.Entity<Order>()
                .HasMany(x => x.Accidents)
                .WithOne(x => x.Order);

            builder.Entity<Accident>()
                .HasMany(x => x.PhotoDocumantation)
                .WithOne(x => x.Accident);

            base.OnModelCreating(builder);


            //TODO: add applicationUser

            List<IdentityRole> roles = new List<IdentityRole> {
                new IdentityRole { Id = "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "0b5141f7-3aed-4cf9-a51d-4ad671703e1f", Name = "Customer", NormalizedName = "CUSTOMER" }
            };
            builder.Entity<IdentityRole>().HasData(roles);

            var pH = new PasswordHasher<IdentityUser>();
            var user = new IdentityUser
            {
                Id = "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406",
                Email = "vilem.cech@gmail.com",
                EmailConfirmed = true,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                NormalizedEmail = "VILEM.CECH@GMAIL.COM",
                PhoneNumber = "773951604",
                PhoneNumberConfirmed = true,
            };
            user.PasswordHash = pH.HashPassword(user, "yo");
            builder.Entity<IdentityUser>().HasData(new List<IdentityUser> { user });

            var userRoles = new IdentityUserRole<string>
            {
                RoleId = "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
                UserId = "1b7fe7c6-fc40-4f0e-934e-7c83f9d75406"
            };
            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);

            // roleClaims
            //var roleClaims = new List<IdentityRoleClaim<string>> {
            //    new IdentityRoleClaim<string>
            //    {
            //        RoleId = "b49e5e21-bcdb-4fac-b8ea-bfa2d81168f7",
            //        Id= 1,
            //        ClaimType= ClaimTypes.Role,
            //        ClaimValue = "admin"
            //    },
            //    new IdentityRoleClaim<string>
            //    {
            //        RoleId = "0b5141f7-3aed-4cf9-a51d-4ad671703e1f",
            //        Id= 2,
            //        ClaimType= ClaimTypes.Role,
            //        ClaimValue = "customer"
            //    }
            //};
            //builder.Entity<IdentityRoleClaim<string>>().HasData(roleClaims);

            builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);
        }
    }
}
