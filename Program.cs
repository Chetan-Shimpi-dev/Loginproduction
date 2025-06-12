using LoginSystem;
using LoginSystem.Data;
using LoginSystem.MailSending;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // JWT Authentication Configuration

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<CustomAuthentication>();
    builder.Services.AddTransient<IEmailService, MailService>();

    //builder.Services.AddAuthentication().AddJwtBearer(options =>
    //{
    //    options.TokenValidationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuer = true,
    //        ValidateAudience = true,
    //        ValidateLifetime = true,
    //        ValidateIssuerSigningKey = true,
    //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
    //        ValidAudience = builder.Configuration["Jwt:Audience"],
    //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    //    };
    //});

    // Authorization Services
    builder.Services.AddAuthorization();

    // Add services to the container
    var connectionString = builder.Configuration.GetConnectionString("AppDb") ?? throw new InvalidOperationException("Connection string 'AppDb' not found.");
    builder.Services.AddDbContext<AppdbContext>(options =>
        options.UseSqlServer(connectionString));

    // Session and Caching Configuration
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

    // MVC and Razor Views
    builder.Services.AddControllersWithViews();
    builder.Services.AddMvc();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts(); // Adds Strict-Transport-Security header
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseSession();
    app.UseRouting();

    // Authentication and Authorization Middleware
    app.UseAuthentication();
    app.UseAuthorization();

    // Default route
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=LoginIndex}");

    app.MapFallbackToController("LoginIndex", "Login");
    app.Run();
}
catch (Exception e)
{
    // Handle any errors here (e.g., log the error)
    Console.WriteLine(e.Message);
}

