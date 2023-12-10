using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Business.DataSources;
using Business.DataStores;
using Business.Models;
using Leupold.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api;

public class Startup
{
    private ServiceProvider _serivceProvider;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var salt = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("APP_SALT") ?? Configuration["App:Salt"]);
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
#if DEBUG
                x.RequireHttpsMetadata = false;
#endif
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(salt),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated,
                    OnAuthenticationFailed = OnAuthenticationFailed
                };
            });

#if DEBUG
        services.AddSwaggerDocumentation();
#endif

        services.AddControllers(options => { options.InputFormatters.Insert(0, GetJsonPatchInputFormatter()); })
            .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = null; });
        ConfigureAppSettings(services);
    }

    private NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
    {
        var builder = new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services.BuildServiceProvider();

        return builder
            .GetRequiredService<IOptions<MvcOptions>>()
            .Value
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();
    }

    private void ConfigureAppSettings(IServiceCollection services)
    {
        services.AddSingleton<IApiSettings>(new ApiSettings
        {
            Environment = Environment.GetEnvironmentVariable("APP_ENVIRONMENT") ?? Configuration["App:Environment"],
            DB_Connection =
                Environment.GetEnvironmentVariable("APP_DBCONNECTION") ?? Configuration["App:DB_Connection"],
            Salt = Environment.GetEnvironmentVariable("APP_SALT") ?? Configuration["App:Salt"],
            SysAccountID = Environment.GetEnvironmentVariable("APP_SYSACCOUNTID") ?? Configuration["App:SysAccountID"],
            WeatherAPI = Environment.GetEnvironmentVariable("OPENWEATHER_API") ?? Configuration["OpenWeather:Api"],
            WeatherSecret = Environment.GetEnvironmentVariable("OPENWEATHER_SECRET") ??
                            Configuration["OpenWeather:Secret"],
            Version = Environment.GetEnvironmentVariable("APP_VERSION") ?? Configuration["App:Version"]
        });

        services.AddTransient<IDatabase, MySqlDatabase>();
        services.AddTransient<IAppDataSource, AppDataSource>();
        services.AddEntityControllers();

        _serivceProvider = services.BuildServiceProvider();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
        // global cors policy
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        //.AllowCredentials());

        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();

        app.UseStaticFiles();

#if DEBUG

        app.UseSwaggerDocumentation().UseAuthentication();
#endif

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private Task OnAuthenticationFailed(AuthenticationFailedContext context)
    {
        return Task.CompletedTask;
    }

    private Task OnTokenValidated(TokenValidatedContext context)
    {
        if (context.SecurityToken is SecurityToken token && context.Principal.Identity is ClaimsIdentity identity)
        {
            var db = _serivceProvider.GetService<IDatabase>();
            var dbUser = db.QuerySingle(DataStoreConfig.CreateGet<User>("ID", identity.Name));
            if (dbUser != null)
            {
                var settings = _serivceProvider.GetService<IApiSettings>();
                settings.CurrentUser = dbUser.ToObject<User>();
            }
        }

        return Task.CompletedTask;
    }
}