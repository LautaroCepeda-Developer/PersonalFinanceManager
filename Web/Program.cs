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
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration["DefaultConnection"] ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews(options => 
{
    // Global authorization policy to require authenticated users by default
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

    options.Filters.Add(new AuthorizeFilter(policy));
})
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options => options.DataAnnotationLocalizerProvider = (type, factory) =>
    {
        return factory.Create("ValidationMessages", typeof(Program).Assembly.FullName!);
    });

# region Rate Limiting Setup
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429; // Too Many Requests

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;

        var localizer =
            context.HttpContext.RequestServices
                .GetRequiredService<IStringLocalizer<SecurityMessages>>();

        await context.HttpContext.Response.WriteAsync(
            localizer["TooManyRequests"],
            cancellationToken: token
        );
    };

    // Export limiter
    options.AddFixedWindowLimiter("export", limiterOptions =>
    {
        limiterOptions.PermitLimit = 3;
        limiterOptions.Window = TimeSpan.FromMinutes(1);

        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        limiterOptions.QueueLimit = 0; // No queuing, reject immediately when limit is reached
    });

    // Expense creation limiter
    options.AddFixedWindowLimiter("expense-create", limiterOptions =>
    {
        limiterOptions.PermitLimit = 15;
        limiterOptions.Window = TimeSpan.FromMinutes(1);

        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        limiterOptions.QueueLimit = 0; // No queuing, reject immediately when limit is reached
    });

    // Expense edit limiter
    options.AddFixedWindowLimiter("expense-edit", limiterOptions =>
    {
        limiterOptions.PermitLimit = 15;
        limiterOptions.Window = TimeSpan.FromMinutes(1);

        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        limiterOptions.QueueLimit = 0; // No queuing, reject immediately when limit is reached
    });

    // Expense delete limiter
    options.AddFixedWindowLimiter("expense-delete", limiterOptions =>
    {
        limiterOptions.PermitLimit = 20;
        limiterOptions.Window = TimeSpan.FromMinutes(1);

        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        limiterOptions.QueueLimit = 0; // No queuing, reject immediately when limit is reached
    });

    // Login limiter
    options.AddFixedWindowLimiter("login", limiterOptions => 
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);

        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        limiterOptions.QueueLimit = 0; // No queuing, reject immediately when limit is reached
    });

    // Register limiter
    options.AddFixedWindowLimiter("register", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);

        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

        limiterOptions.QueueLimit = 0; // No queuing, reject immediately when limit is reached
    });
});
#endregion

#region Additional security setup (Headers & Identity options)
// Cookie settings for authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;

    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.Cookie.SameSite = SameSiteMode.Strict;

    options.SlidingExpiration = true;

    options.ExpireTimeSpan = TimeSpan.FromDays(14);
});

// Identity options configuration for account lockout
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
});
#endregion

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
builder.Services.AddScoped<IExpenseExportService, ExpenseExportService>();

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


app.UseRateLimiter();

app.UseAuthorization();

// Middleware to add security headers to all responses
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    await next();
});

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
