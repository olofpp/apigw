using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ITest;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using GetJwks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IHttpConfiguration, HttpConfiguration>();

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddYamlFile("appsettings.yml", optional: true, reloadOnChange: true)
    .Build();

JsonWebKey jsonResponse = await new HttpConfiguration(configuration).ReturnMessage();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = jsonResponse
        };
    });

builder.Services.AddEndpointsApiExplorer();



// Rate limit policys
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("rate-10", options =>
    {
        options.Window = TimeSpan.FromSeconds(60);
        options.PermitLimit = 10;
    });
    rateLimiterOptions.AddFixedWindowLimiter("rate-100", options =>
    {
        options.Window = TimeSpan.FromSeconds(60);
        options.PermitLimit = 100;
    });
    rateLimiterOptions.AddFixedWindowLimiter("rate-1k", options =>
    {
        options.Window = TimeSpan.FromSeconds(60);
        options.PermitLimit = 1000;
    });
    rateLimiterOptions.AddFixedWindowLimiter("rate-5k", options =>
    {
        options.Window = TimeSpan.FromSeconds(60);
        options.PermitLimit = 5000;
    });
    rateLimiterOptions.AddFixedWindowLimiter("rate-10k", options =>
    {
        options.Window = TimeSpan.FromSeconds(60);
        options.PermitLimit = 10000;
    });
});



builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();
