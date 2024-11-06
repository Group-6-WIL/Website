using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Initialize Firebase Admin SDK only once
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(@"C:\Users\reesa\Wil_v3\ladybird-foundation-firebase-adminsdk-2m5sm-69f5024839.json")
        });

        // Add services to the container
        builder.Services.AddControllersWithViews();

        // Add authentication using Firebase JWT Bearer
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://securetoken.google.com/ladybird-foundation"; // Replace with your Firebase project ID
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://securetoken.google.com/ladybird-foundation", // Your Firebase project ID
                    ValidateAudience = true,
                    ValidAudience = "ladybird-foundation", // Your Firebase project ID
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                    {
                        // Get Firebase public keys for signature validation
                        return new JsonWebKeySet("https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com").Keys;
                    }
                };
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Enable Firebase authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}