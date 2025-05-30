using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using ReservationSystemWebAPI.DataAccess;
using ReservationSystemWebAPI.Models;
using ReservationSystemWebAPI.Repositories;
using ReservationSystemWebAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string for the database from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Bind JWT configuration settings from appsettings.json to JwtSettings object
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    // Set default authentication and challenge scheme to JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        // Audience validation is disabled for development because frontend runs on multiple localhost ports
        // ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(key),

        // Custom audience validation to allow multiple localhost ports during development
        AudienceValidator = (audiences, securityToken, validationParameters) =>
        {
            // Accept token if any audience claim starts with "https://localhost" or "http://localhost"
            return audiences.Any(aud => aud.StartsWith("https://localhost") || aud.StartsWith("http://localhost"));
        }
    };
});

// Add Authorization services to the container
builder.Services.AddAuthorization();

// Configure CORS policy to allow all origins, methods, and headers (for development/testing)
// This is needed to enable the frontend application, which may be hosted on a different origin (domain or port),
// to successfully make cross-origin HTTP requests to this API without being blocked by the browser’s same-origin policy.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});


// Register Entity Framework Core DbContext with SQL Server provider using the connection string
builder.Services.AddDbContext<ReservationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories and services for dependency injection
builder.Services.AddScoped<IStorageItemRepository, StorageItemRepository>();
builder.Services.AddScoped<IStorageItemService, StorageItemService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Add MVC Controllers support
builder.Services.AddControllers();

// Register Swagger/OpenAPI generator for API documentation
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to use JWT Authentication in the UI
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your JWT token.\r\n\r\nExample: \"Bearer eyJhb...\"",
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure middleware pipeline for development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // Enable Swagger middleware
    app.UseSwaggerUI();     // Enable Swagger UI
}

app.UseHttpsRedirection();   // Redirect HTTP requests to HTTPS

app.UseCors("AllowAll");     // Apply CORS policy to allow all origins, methods, and headers

app.UseAuthentication();     // Enable authentication middleware

app.UseAuthorization();      // Enable authorization middleware

app.MapControllers();        // Map controller endpoints

app.Run();                   // Run the web application