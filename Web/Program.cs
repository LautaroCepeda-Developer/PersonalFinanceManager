using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using Web;
using Web.Data;
using Web.Mapping;
using Web.Repositories.Categories;
using Web.Repositories.Expenses;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration["DefaultConnection"] ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options => options.DataAnnotationLocalizerProvider = (type, factory) =>
    {
        return factory.Create("ValidationMessages", typeof(Program).Assembly.FullName!);
    });


#region Localization Setup
builder.Services.AddLocalization(options => {
    options.ResourcesPath = "Resources";
});

builder.Services.AddSingleton<IConfigureOptions<MvcOptions>, MvcLocalizationConfiguration>();

builder.Services.Configure<RequestLocalizationOptions>(options => {
    options.SetDefaultCulture(Web.Utilities.CultureConstants.SupportedCultures[0])
        .AddSupportedCultures(Web.Utilities.CultureConstants.SupportedCultures)
        .AddSupportedUICultures(Web.Utilities.CultureConstants.SupportedCultures);
    options.AddInitialRequestCultureProvider(new AcceptLanguageHeaderRequestCultureProvider());
});

#endregion

#region External Authentication Providers Setup
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google client id not found.");
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Google client secret not found.");
});

builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? throw new InvalidOperationException("Facebook app id not found.");
    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? throw new InvalidOperationException("Facebook app secret not found.");
});
#endregion

#region Dependency Injection
// Auto Mapper Configurations
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

// Expenses
builder.Services.AddScoped<IExpenseRepository, EfExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

// Categories
builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
