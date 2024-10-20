using FluentValidation.AspNetCore;
using LilamiBazzar.Models.Models;
using Microsoft.EntityFrameworkCore;
using LilamiBazzar.DataAccess.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LilamiBazzar.Services;
using LilamiBazzar.Services.PasswordHashingService;
using LilamiBazzar.DataAccess.DbInitializer;
using LilamiBazzar.Utility.Services.EmailService;
using LilamiBazzar.Services.CheckAndCompleteAuction;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Add FluentValidation services using the new recommended methods
        builder.Services.AddFluentValidationAutoValidation()
                        .AddFluentValidationClientsideAdapters();

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        builder.Services.AddScoped<IDbInitializer, DbInitializer>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddHostedService<CheckAndCompleteAuction>();
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                /* ValidIssuer = builder.Configuration["JWT:ValidIssuer"],*/
                /* ValidAudience = builder.Configuration["JWT:ValidAudience"],*/
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["Authorization"];
                    return Task.CompletedTask;
                }
            };
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        /*app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            await next();
        });*/

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        SeedDatabase();

        app.MapControllerRoute(
            name: "default",
            pattern: "{area=Users}/{controller=Home}/{action=Index}/{id?}");


        app.Run();

        void SeedDatabase()
        {
            using (var scope = app.Services.CreateScope())
            {

                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                dbInitializer.Initialize();
            }
        }
    }
}
