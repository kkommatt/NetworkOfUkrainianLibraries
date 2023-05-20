using NetworkOfLibrariesWebApplication;
using Microsoft.EntityFrameworkCore;
using NetworkOfLibrariesWebApplication.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NetworkOfLibrariesWebApplication.Infrastructure.Identity.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DbnetworkOfLibrariesContext>(option => option.UseSqlServer(
 builder.Configuration.GetConnectionString("DefaultConnection")
 ));

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationIdentityContext>();

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationIdentityContext>();

builder.Services.AddDbContext<ApplicationIdentityContext>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ApplicationIdentityContext)), sqlOptions => sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name)));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationIdentityContext>();


var app = builder.Build();
await app.InitializeRolesAsync();

/*
var superUserSection = app.Configuration.GetSection("IdentityDefaults:SuperUser");
var defaultUsersSection = app.Configuration.GetSection("IdentityDefaults:DefaultUsers");

var superUserConfiguration = new ConfigurationBuilder()
    .AddInMemoryCollection(new[]
    {
        new KeyValuePair<string, string>("Username", superUserSection.GetValue<string>("Username")),
        new KeyValuePair<string, string>("Password", superUserSection.GetValue<string>("Password"))
    })
    .Build();

var defaultUsersConfiguration = new ConfigurationBuilder()
    .AddInMemoryCollection(new[]
    {
        new KeyValuePair<string, string>("Username", defaultUsersSection.GetValue<string>("Username")),
        new KeyValuePair<string, string>("Password", defaultUsersSection.GetValue<string>("Password"))
    })
    .Build();

await app.InitializeDefaultUsersAsync(superUserConfiguration, defaultUsersConfiguration); */

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

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Libraries}/{action=Index}/{id?}");
app.Run();


