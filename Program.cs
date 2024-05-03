using ArtGalaxy.Data;
using ArtGalaxy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
            name: "view",
            pattern: "View/{type}/{id}",
            defaults: new { controller = "Portfolio", action = "View" }
        );

app.MapControllerRoute(
            name: "like",
            pattern: "/Portfolio/Like",
            defaults: new { controller = "Portfolio", action = "Like" }
        );

app.MapControllerRoute(
            name: "unlike",
            pattern: "/Portfolio/Unlike",
            defaults: new { controller = "Portfolio", action = "Unlike" }
        );

app.MapControllerRoute(
            name: "likeComment",
            pattern: "/Portfolio/LikeComment",
            defaults: new { controller = "Portfolio", action = "LikeComment" }
        );

app.MapControllerRoute(
            name: "unlikeComment",
            pattern: "/Portfolio/UnlikeComment",
            defaults: new { controller = "Portfolio", action = "UnlikeComment" }
        );

app.MapControllerRoute(
            name: "deleteComment",
            pattern: "/Portfolio/DeleteComment",
            defaults: new { controller = "Portfolio", action = "DeleteComment" }
        );

app.MapControllerRoute(
            name: "portfolio",
            pattern: "Portfolio/{name}",
            defaults: new { controller = "Portfolio", action = "Index" }
        );



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "logout",
    pattern: "{controller=Account}/{action=Logout}/{id?}");

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapFallbackToPage("/Home");
});

app.Run();
