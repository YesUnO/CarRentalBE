using Microsoft.Extensions.DependencyInjection;
using DataLayer;
using Microsoft.Extensions.Configuration;
using Core.Domain.Cars;
using Core.Domain.User;
using Core.Infrastructure.Files;
using Core.Domain.Orders;
using Core.Infrastructure.Files.Options;
using Core.Domain.Payment.Options;
using Core.Domain.StripePayments;
using Core.Domain.StripePayments.Interfaces;
using Core.Infrastructure.Emails;
using Core.Infrastructure.Emails.Options;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Newtonsoft.Json;
using Core.Infrastructure.ExternalAuthProviders.Options;
using Core.Infrastructure.Authentication;

namespace Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataLayer(configuration);

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<IOrderService, OrderService>();

        services.AddScoped<IStripeInvoiceService, StripeInvoiceService>();
        services.AddScoped<IStripeWebhookService, StripeWebhookService>();
        services.AddScoped<IStripeProductService, StripeProductService>();

        services.AddScoped<IStripeSubscriptionService, StripeSubscriptionService>();
        services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));

        services.AddScoped<IFileService, FileService>();
        services.Configure<AzureStorageConfig>(configuration.GetSection("AzureStorageConfig"));

        services.AddSingleton<IEmailService, EmailService>();
        services.Configure<EmailsSettings>(configuration.GetSection("EmailsSettings"));

        var googleStorageServiceAccKey = JsonConvert.SerializeObject(configuration.GetSection("GoogleCloud:ServiceClientAPIKeyJson").Get<GoogleStorageServiceAccKey>());
        services.AddSingleton(_ =>
            {
                var credentials = GoogleCredential.FromJson(googleStorageServiceAccKey);
                var googleStorageClient = StorageClient.Create(credentials);
                return googleStorageClient;
            }
        );

        services.Configure<ExternalAuthProvidersConfig>(configuration.GetSection("ExternalAuthenticationProviders"));

        services.AddBffToContainer();

        return services;
    }
}