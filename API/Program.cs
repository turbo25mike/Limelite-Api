//using Microsoft.AspNetCore;
//using Microsoft.AspNetCore.Hosting;

//namespace Api;

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        CreateWebHostBuilder(args).Build().Run();
//    }

//    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
//    {
//        return WebHost.CreateDefaultBuilder(args)
//            .UseStartup<Startup>();
//    }
//}

using System.Security.Claims;
using Api.Utilities;
using Business.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var settings = new ApiSettings
{
    Environment = Environment.GetEnvironmentVariable("APP_ENVIRONMENT") ?? builder.Configuration["App:Environment"],
    DB_Connection = Environment.GetEnvironmentVariable("APP_DBCONNECTION") ?? builder.Configuration["App:DB_Connection"],
    Auth0Audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE") ?? builder.Configuration["Auth0:Audience"],
    Auth0Domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN") ?? builder.Configuration["Auth0:Domain"],
    SysAccountID = Environment.GetEnvironmentVariable("APP_SYSACCOUNTID") ?? builder.Configuration["App:SysAccountID"],
    Version = Environment.GetEnvironmentVariable("APP_VERSION") ?? builder.Configuration["App:Version"]
};

// Add services to the container.
//builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
builder.Services.AddSingleton<IApiSettings>(settings);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.InputFormatters.Insert(0, JsonPatchFormatter.GetJsonPatchInputFormatter());
});
builder.Services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddSwaggerGen();
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("read:messages", policy => policy.Requirements.Add(new
//    HasScopeRequirement("read:messages", domain)));
//});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = $"https://{settings.Auth0Domain}/";
    options.Audience = settings.Auth0Audience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();