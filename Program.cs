using System;
using System.Text;
using ApplicationDBContext.Api.Data;
using SimplyTrack.Api.Models;
using SimplyTrack.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// --- Read DB config from environment variables (fall back to appsettings.json) ---
string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? configuration.GetValue<string>("ConnectionStrings:Host") ?? "localhost";
string dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? configuration.GetValue<string>("ConnectionStrings:Port") ?? "3306";
string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? configuration.GetValue<string>("ConnectionStrings:Database") ?? "simplytrack";
string dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? configuration.GetValue<string>("ConnectionStrings:User") ?? "root";
string dbPass = Environment.GetEnvironmentVariable("DB_PASS") ?? configuration.GetValue<string>("ConnectionStrings:Password") ?? "Your_password";

// Build connection string expected by Pomelo / MySql
var conn = $"server={dbHost};port={dbPort};database={dbName};user={dbUser};password={dbPass};";

// Add DB context (MariaDB / Pomelo)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

// --- Read JWT config from environment variables (fall back to appsettings.json) ---
string jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? configuration["Jwt:Key"];
string jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? configuration["Jwt:Issuer"];
string jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? configuration["Jwt:Audience"];
string accessTokenMinutesStr = Environment.GetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_MINUTES") ?? configuration["Jwt:AccessTokenExpirationMinutes"];
string refreshTokenDaysStr = Environment.GetEnvironmentVariable("REFRESH_TOKEN_DAYS") ?? configuration["Jwt:RefreshTokenDays"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT_KEY must be provided via environment variable or appsettings.json.");
}

if (!int.TryParse(accessTokenMinutesStr, out var accessTokenMinutes))
    accessTokenMinutes = 15;

if (!int.TryParse(refreshTokenDaysStr, out var refreshTokenDays))
    refreshTokenDays = 30;

var jwtKeyBytes = Encoding.UTF8.GetBytes(jwtKey);

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
        ClockSkew = TimeSpan.FromSeconds(30),
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Make Access/Refresh lifetime available to services via configuration or DI if needed
builder.Services.Configure<JwtOptions>(options =>
{
    options.Key = jwtKey;
    options.Issuer = jwtIssuer;
    options.Audience = jwtAudience;
    options.AccessTokenExpirationMinutes = accessTokenMinutes;
    options.RefreshTokenDays = refreshTokenDays;
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISetService, SetService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "SimplyTrack API", 
        Version = "v1",
        Description = "A comprehensive fitness tracking API with JWT authentication. Track exercises, workout sessions, and individual sets with detailed analytics.",
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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

    // Enable XML documentation comments
    var xmlFile = "SimplyTrack-API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, true);
    }
    
    // Group endpoints by controller
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.DocInclusionPredicate((name, api) => true);
});

var app = builder.Build();

// Apply EF Core migrations at startup (optional convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Enable Swagger in all environments (useful for API documentation)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimplyTrack API v1");
    c.RoutePrefix = "swagger"; // Keep swagger at /swagger
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Simple POCO used for DI configuration of JWT settings
public class JwtOptions
{
    public string Key { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenDays { get; set; }
}