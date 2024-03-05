using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WIL_Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // AddRoles here to register RoleManager
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Create Admin role and assign the default user to it
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Ensure the "Admin" role exists, and create it if not
    if (!roleManager.RoleExistsAsync("Admin").Result)
    {
        IdentityRole adminRole = new IdentityRole("Admin");
        IdentityResult result = roleManager.CreateAsync(adminRole).Result;

        if (!result.Succeeded)
        {
            // Handle error (log or throw exception)
            Console.WriteLine("Error creating Admin role.");
        }
    }

    // Assign the default user to the "Admin" role
    IdentityUser defaultUser = userManager.FindByEmailAsync("admin1@gmail.com").Result;
    if (defaultUser != null)
    {
        userManager.AddToRoleAsync(defaultUser, "Admin").Wait();
    }
    else
    {
        Console.WriteLine("Default user not found.");
    }
}

app.Run();
