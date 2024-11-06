using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder; // Add this using directive
using Microsoft.AspNetCore.Hosting; // Add this using directive
using Microsoft.Extensions.DependencyInjection; // Add this using directive
using Microsoft.Extensions.Hosting; // Add this using directive
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Initialize Firebase Admin SDK
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(@"C:\Users\reesa\Downloads\Wil_v3\Wil_v3\Wil_v3\ladybird-foundation-firebase-adminsdk-2m5sm-69f5024839.json")
        });

        // Add services to the container
        builder.Services.AddControllersWithViews();

        // Add session services
        builder.Services.AddDistributedMemoryCache(); // Enables caching
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
            options.Cookie.HttpOnly = true; // Make the cookie HTTP only
            options.Cookie.IsEssential = true; // Make the session cookie essential
        });

        // Add Firebase JWT Bearer Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://securetoken.google.com/ladybird-foundation"; // Replace with your Firebase project ID
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://securetoken.google.com/ladybird-foundation",
                    ValidateAudience = true,
                    ValidAudience = "ladybird-foundation",
                    ValidateLifetime = true
                };
            });

        var app = builder.Build();

        // Middleware configuration
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // Use session middleware
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
