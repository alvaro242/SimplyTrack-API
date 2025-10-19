# SimplyTrack API - AI Coding Assistant Instructions

## Project Overview
SimplyTrack is a fitness tracking API built with ASP.NET Core 8, targeting workout session management with JWT authentication and MySQL/MariaDB persistence.

## Architecture & Core Patterns

### Domain Model
- **Exercise → Session → SetEntity** - Hierarchical fitness tracking structure
- Exercise contains multiple Sessions (workout instances), each Session contains multiple Sets
- `ApplicationUser` extends Identity with `FirstName`/`LastName` properties
- All entities use string GUIDs as primary keys (except `RefreshToken` uses int)

### Configuration Priority Pattern
Environment variables override `appsettings.json` values throughout the application:
```csharp
string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? configuration.GetValue<string>("ConnectionStrings:Host") ?? "localhost";
```
This pattern is used for DB connection, JWT settings, and token lifetimes.

### Namespace Conventions
- **Assembly**: `SimplyTrackAPI` (no hyphens)
- **Root Namespace**: `SimplyTrack.Api` 
- **DbContext Namespace**: `ApplicationDBContext.Api.Data` (note: different from root)
- Models in `SimplyTrack.Api.Models`, Services in `SimplyTrack.Api.Services`

### Database & Migrations
- **Pomelo MySQL/MariaDB** provider with auto-server detection
- **Auto-migration on startup** in `Program.cs` using scoped service
- Cascade delete configured: Exercise → Sessions → Sets
- Unique index on `RefreshToken.Token`

### Authentication Architecture
- **ASP.NET Core Identity** with custom `ApplicationUser`
- **JWT Bearer** with refresh token rotation pattern
- Token services use DI-configured `JwtOptions` class for lifetime management
- Refresh tokens track IP addresses, revocation chains, and expiration

## Development Workflow

### Running the Application
```bash
dotnet run --project SimplyTrack-API.csproj
```
- **Dev ports**: HTTP 5032, HTTPS 7262
- **Swagger UI** available at `/swagger` in development
- Auto-applies EF migrations on startup

### Database Setup
Configure via environment variables or `appsettings.json`:
- `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASS`
- Default: `localhost:3306`, database `simplytrack`, user `root`

### JWT Configuration
Required environment variables (or appsettings):
- `JWT_KEY` (minimum 32 characters) - **Required**
- `JWT_ISSUER`, `JWT_AUDIENCE`
- `ACCESS_TOKEN_EXPIRATION_MINUTES`, `REFRESH_TOKEN_DAYS`

## Code Patterns & Conventions

### Entity Patterns
- String GUID primary keys: `public string Id { get; set; } = Guid.NewGuid().ToString();`
- UTC timestamps: `public DateTime CreatedAt { get; set; } = DateTime.UtcNow;`
- Navigation properties for EF relationships

### Service Layer
- Repository pattern for data access (`IRefreshTokenRepository`, `RefreshTokenRepository`)
- Service interfaces in Models folder (`ITokenService`)
- Implementations in Services folder

### Error Handling
- Throws `InvalidOperationException` for missing required configuration
- Repository methods handle null gracefully (see `RevokeAsync`)

## Key Files Reference
- **`Program.cs`**: Configuration, DI setup, auto-migration, JWT/Identity setup
- **`Data/ApplicationDbContext.cs`**: EF models, relationships, unique constraints
- **`Models/`**: Domain entities and service interfaces
- **`Services/`**: Repository and service implementations
- **`appsettings.json`**: Default configuration (override with env vars)
- **`SimplyTrack-API.http`**: API testing file for development

## Testing & Debugging
- Use `.http` files in VS Code with REST Client extension
- Swagger UI provides interactive API documentation in development
- EF migrations auto-apply on startup for rapid iteration