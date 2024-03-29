using CarRentalAPI;
using Core.Infrastructure.Options;
using Microsoft.OpenApi.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

var apiUrls = builder.WebHost.GetSetting(WebHostDefaults.ServerUrlsKey).Split(";");
var baseApiUrls = new BaseApiUrls
{
    HttpsUrl = apiUrls.FirstOrDefault(x => x.StartsWith("https:")),
    HttpUrl = apiUrls.FirstOrDefault(x => x.StartsWith("http:"))
};
builder.Services.AddOptions<BaseApiUrls>()
    .Configure(x =>
    {
        x.HttpsUrl = baseApiUrls.HttpsUrl;
        x.HttpUrl = baseApiUrls.HttpUrl;
        x.FrontEndUrl = builder.Configuration["FrontEndUrl"];

    });

builder.Services.AddThingsToContainer(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            new string[] {}
        },
    });
});

var app = builder.Build();
var stripeApiKey = configuration.GetSection("StripeSettings:ApiKey").Value;

StripeConfiguration.ApiKey = stripeApiKey;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.OAuthClientId("my-client-id");
        c.OAuthClientSecret("my-client-secret");
        c.OAuthAppName("My App");
        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseBff();
app.UseAuthorization();
app.UseCors("CorsPolicy");

app.MapBffManagementEndpoints();
app.MapControllers();

app.Run();
