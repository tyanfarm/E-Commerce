using System.Configuration;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using AspNetCoreHero.ToastNotification;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// DbContext
var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));
builder.Services.AddDbContext<EcommerceContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), serverVersion);
});

// Add Session
builder.Services.AddSession();

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(p => {
                    p.LoginPath = "/login.html";
                    p.AccessDeniedPath = "/";
                });

// Program Configuration understand Unicode text
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

// AddRazorRuntimeCompilation - F5 to reload Razor
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// AspNetCoreHero.ToastNotification
builder.Services.AddNotyf(config => {config.DurationInSeconds=3; config.IsDismissable=true; config.Position=NotyfPosition.TopRight;} );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
