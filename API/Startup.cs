//using System;
//using System.Security.Claims;
//using Api.Utilities;
//using Business.DataSources;
//using Business.DataStores;
//using Business.Models;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.IdentityModel.Tokens;

//namespace Api;

//public class Startup
//{
//    private ApiSettings _Settings;

//    public Startup(IConfiguration configuration)
//    {
//        Configuration = configuration;
//    }

//    public IConfiguration Configuration { get; }

//    // This method gets called by the runtime. Use this method to add services to the container.
//    public void ConfigureServices(IServiceCollection services)
//    {
//        _Settings = new ApiSettings
//        {
//            Environment = Environment.GetEnvironmentVariable("APP_ENVIRONMENT") ?? Configuration["App:Environment"],
//            DB_Connection = Environment.GetEnvironmentVariable("APP_DBCONNECTION") ?? Configuration["App:DB_Connection"],
//            Auth0Audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE") ?? Configuration["Auth0:Audience"],
//            Auth0Domain = Environment.GetEnvironmentVariable("AUTH0_DOMAIN") ?? Configuration["Auth0:Domain"],
//            SysAccountID = Environment.GetEnvironmentVariable("APP_SYSACCOUNTID") ?? Configuration["App:SysAccountID"],
//            Version = Environment.GetEnvironmentVariable("APP_VERSION") ?? Configuration["App:Version"]
//        };

//        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//        .AddJwtBearer(options =>
//        {
//            options.Authority = _Settings.Auth0Domain;
//            options.Audience = _Settings.Auth0Audience;
//            options.TokenValidationParameters = new TokenValidationParameters
//            {
//                NameClaimType = ClaimTypes.NameIdentifier
//            };
//        });

//        services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//        services.AddSwaggerGen();

//        services.AddControllers(options =>
//        {
//            options.Filters.Add<ExceptionFilter>();
//            options.InputFormatters.Insert(0, JsonPatchFormatter.GetJsonPatchInputFormatter());
//        });

//        services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
//        services.AddSingleton<IApiSettings>(_Settings);
//        services.AddTransient<IDatabase, MySqlDatabase>();
//        services.AddTransient<IAppDataSource, AppDataSource>();
//        services.AddEntityControllers();
//    }

//    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//    public void Configure(IApplicationBuilder app, IHostEnvironment env)
//    {
//        // global cors policy
//        app.UseCors(x => x
//            .AllowAnyOrigin()
//            .AllowAnyMethod()
//            .AllowAnyHeader());
//        //.AllowCredentials());

//        if (env.IsDevelopment())
//            app.UseDeveloperExceptionPage();
//        //else
//        //    app.UseHsts();

//        app.UseAuthentication();
//        app.UseRouting();
//        app.UseAuthorization();

//        app.UseStaticFiles();

//#if DEBUG
//        app.UseSwagger();
//        app.UseSwaggerUI();
//#endif

//        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
//    }
//}