using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using new_project_register.Areas.Identity.Data;
using new_project_register.Data;
using Rotativa.AspNetCore;
using static System.Formats.Asn1.AsnWriter;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("new_project_registerContextConnection") ?? throw new InvalidOperationException("Connection string 'new_project_registerContextConnection' not found.");

builder.Services.AddDbContext<new_project_registerContext>(options => options.UseSqlServer(connectionString));

IdentityBuilder identityBuilder = builder.Services.AddDefaultIdentity<new_project_registerUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<new_project_registerContext>();
// Add services to the container.
builder.Services.AddControllersWithViews();
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User_Airline}/{action=Index}/{id?}");
 app.MapRazorPages();

async Task EnsureRolesAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { "Admin", "Manager", "Member" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}

async Task EnsureDefaultUserAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<new_project_registerUser>>();
        string adminEmail = "ahmed@gmail.com";
        string adminPassword = "Ahmed123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var user = new new_project_registerUser { Email = adminEmail, UserName = adminEmail };
            var result = await userManager.CreateAsync(user, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}

app.Run();
