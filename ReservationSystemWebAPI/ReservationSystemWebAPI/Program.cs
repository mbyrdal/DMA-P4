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
// Located under "ConnectionStrings:DefaultConnection":
// Example: "Server=hildur.ucn.dk;Database=DMA-CSD-S235_10632110;User Id=DMA-CSD-S235_10632110;Password=Password1!;TrustServerCertificate=true"
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Bind JWT configuration settings from appsettings.json section "JwtSettings" to the JwtSettings object
// JwtSettings example from appsettings.json:
// {
//   "SecretKey": "ThisIsASecretKeyThatIsAtLeast32Chars!",
//   "Issuer": "https://localhost:7092",
//   "Audience": "https://localhost",
//   "ExpiryMinutes": 60
// }
// Note: 
// - "SecretKey" should be a sufficiently long and secure secret used to sign JWT tokens.
// - "Issuer" is the token issuer URL (here pointing to your local dev server).
// - "Audience" is the intended recipient of the token (here set loosely to localhost for dev).
// - "ExpiryMinutes" configures the token lifetime.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Configure JWT Authentication using values from JwtSettings
builder.Services.AddAuthentication(options =>
{
    // Set JWT Bearer as the default authentication and challenge scheme
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,                       // Ensure the token issuer matches JwtSettings.Issuer
        ValidateAudience = true,                     // Validate audience claim; customized for localhost below
        ValidateLifetime = true,                     // Ensure token is not expired
        ValidateIssuerSigningKey = true,             // Validate signature with secret key
        ValidIssuer = jwtSettings.Issuer,            // Expected issuer URL

        // Audience validation is customized to allow multiple localhost ports during development.
        // In production, use a strict exact audience URL such as your deployed frontend app URL.
        // Uncomment the line below for production:
        // ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(key),

        // Custom AudienceValidator allowing tokens with audience claims starting with localhost URLs
        AudienceValidator = (audiences, securityToken, validationParameters) =>
        {
            return audiences.Any(aud => aud.StartsWith("https://localhost") || aud.StartsWith("http://localhost"));
        }
    };
});

// Add Authorization services to the container
builder.Services.AddAuthorization();

// Configure CORS policy "AllowAll" to allow any origin, method, and header, enabling
// the frontend running on different domains or ports (like localhost:3000) to access this API without CORS errors.
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