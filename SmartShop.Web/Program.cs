using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SmartShop.Application.Interfaces;
using SmartShop.Application.Services;
using SmartShop.Domain.Entities;
using SmartShop.Infrastructure.DependencyInjection;
using SmartShop.Infrastructure.Identity;
using SmartShop.Infrastructure.Persistence;
using SmartShop.Web.Middleware;
using System.Globalization;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnSigningIn = async context =>
    {
        var userManager = context.HttpContext
            .RequestServices
            .GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.GetUserAsync(context.Principal);

        if (user != null)
        {
            var claimsIdentity = (ClaimsIdentity)context.Principal.Identity!;

            // Old ShopId claimni olib tashlaymiz (agar mavjud bo‘lsa)
            var existingClaim = claimsIdentity.FindFirst("ShopId");
            if (existingClaim != null)
                claimsIdentity.RemoveClaim(existingClaim);

            // Yangi ShopId claim qo‘shamiz
            claimsIdentity.AddClaim(
                new Claim("ShopId", user.ShopId.ToString()));
        }
    };
});


builder.Services.AddRazorPages();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultureCodes = new[] { "uz", "en" };
var supportedCultures = supportedCultureCodes
    .Select(culture => new CultureInfo(culture))
    .ToList();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("uz");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = new IRequestCultureProvider[]
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});
builder.Services
    .AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<ExceptionMiddleware>();

var localizationOptions = app.Services
    .GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>()
    .Value;

app.UseRequestLocalization(localizationOptions);


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

if (app.Environment.IsDevelopment())
{
    await SeedDataAsync(app);
}



app.Run();

static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    const string roleName = "Owner";

    // ================================
    // 1️⃣ CREATE ROLE
    // ================================
    if (!await roleManager.RoleExistsAsync(roleName))
    {
        await roleManager.CreateAsync(new IdentityRole(roleName));
    }

    // ================================
    // 2️⃣ FIRST SHOP
    // ================================
    var firstShop = await dbContext.Shops
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(s => s.Name == "Main Shop");

    if (firstShop == null)
    {
        firstShop = new Shop
        {
            Id = Guid.NewGuid(),
            Name = "Main Shop"
        };

        dbContext.Shops.Add(firstShop);
        await dbContext.SaveChangesAsync();
    }

    const string adminEmail = "admin@mail.com";
    const string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            ShopId = firstShop.Id
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (!result.Succeeded)
            throw new Exception("Failed to create first admin");
    }

    if (!await userManager.IsInRoleAsync(adminUser, roleName))
    {
        await userManager.AddToRoleAsync(adminUser, roleName);
    }

    // ================================
    // 3️⃣ SECOND SHOP
    // ================================
    var secondShop = await dbContext.Shops
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(s => s.Name == "Second Shop");

    if (secondShop == null)
    {
        secondShop = new Shop
        {
            Id = Guid.NewGuid(),
            Name = "Second Shop"
        };

        dbContext.Shops.Add(secondShop);
        await dbContext.SaveChangesAsync();
    }

    const string secondEmail = "admin2@mail.com";
    const string secondPassword = "Admin123!";

    var secondUser = await userManager.FindByEmailAsync(secondEmail);

    if (secondUser == null)
    {
        secondUser = new ApplicationUser
        {
            UserName = secondEmail,
            Email = secondEmail,
            EmailConfirmed = true,
            ShopId = secondShop.Id
        };

        var result = await userManager.CreateAsync(secondUser, secondPassword);

        if (!result.Succeeded)
            throw new Exception("Failed to create second admin");
    }

    if (!await userManager.IsInRoleAsync(secondUser, roleName))
    {
        await userManager.AddToRoleAsync(secondUser, roleName);
    }
}
