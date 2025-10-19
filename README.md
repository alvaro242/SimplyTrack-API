# SimplyTrack API

**SimplyTrack API** is a comprehensive fitness tracking REST API built with ASP.NET Core 8, featuring JWT authentication, refresh tokens, and complete workout management capabilities. Perfect for mobile apps and fitness tracking applications.

## 🏋️ Features

### Authentication & Security
- **JWT Access Tokens** with configurable expiration (default: 15 minutes)
- **Refresh Token System** - rotating, persisted, and revocable (default: 30 days)
- **User Registration & Login** with ASP.NET Core Identity
- **IP Address Tracking** for security auditing
- **Proper Authorization** on all protected endpoints

### Fitness Tracking
- **Exercise Management** - Create, update, delete, and search exercises
- **Workout Sessions** - Track workout sessions with date-based organization
- **Set Recording** - Record reps and weight for each set
- **Automatic Calculations** - Session totals (weight, reps, sets count)
- **Dashboard Views** - Quick overview with last session summaries
- **Exercise History** - Complete workout history per exercise

### Technical Features
- **Environment-first Configuration** - `.env` file support with fallbacks
- **Auto-migrations** - Database schema updates on startup
- **Comprehensive Validation** - Request/response DTOs with validation
- **Enhanced Swagger Documentation** - Interactive API documentation with JWT support
- **RESTful Design** - Following REST conventions
- **User-scoped Data** - All data is isolated per authenticated user
- **Exercise Management** - Create, update, delete, and search exercises
- **Workout Sessions** - Track workout sessions with date-based organization
- **Set Recording** - Record reps and weight for each set
- **Automatic Calculations** - Session totals (weight, reps, sets count)
- **Dashboard Views** - Quick overview with last session summaries
- **Exercise History** - Complete workout history per exercise

### Technical Features
- **Environment-first Configuration** - `.env` file support with fallbacks
- **Auto-migrations** - Database schema updates on startup
- **Comprehensive Validation** - Request/response DTOs with validation
- **OpenAPI/Swagger** - Full API documentation
- **RESTful Design** - Following REST conventions

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL/MariaDB server
- (Optional) [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) VS Code extension for testing

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/alvaro242/SimplyTrack-API.git
   cd SimplyTrack-API
   ```

2. **Configure environment variables**
   
   Create a `.env` file in the project root:
   ```env
   # Database Configuration
   DB_HOST=localhost
   DB_PORT=3306
   DB_NAME=simplytrack
   DB_USER=root
   DB_PASS=your_password

   # JWT Configuration (REQUIRED)
   JWT_KEY=your_super_secure_jwt_key_at_least_32_characters_long
   JWT_ISSUER=SimplyTrack
   JWT_AUDIENCE=SimplyTrackClient
   ACCESS_TOKEN_EXPIRATION_MINUTES=15
   REFRESH_TOKEN_DAYS=30

   # Environment
   ASPNETCORE_ENVIRONMENT=Development
   ```

3. **Install dependencies**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

The API will be available at:
- **HTTP**: http://localhost:5032
- **HTTPS**: https://localhost:7262
- **Swagger UI**: http://localhost:5032/swagger (Development only)

## 📚 Enhanced API Documentation

### Swagger UI Features
- **🔐 JWT Authentication Support** - "Authorize" button for easy token testing
- **📝 Comprehensive Documentation** - Detailed endpoint descriptions and examples
- **🏷️ Organized Categories** - Endpoints grouped by functional areas:
  - Authentication
  - User Management
  - Exercise Management
  - Session Management
  - Set Management
  - Dashboard
- **📊 Response Specifications** - HTTP status codes and response types
- **🔍 Parameter Documentation** - Detailed parameter descriptions and validation rules

### Using Swagger UI
1. Navigate to `http://localhost:5032/swagger`
2. Register or login using `/api/auth/register` or `/api/auth/login`
3. Copy the `accessToken` from the response
4. Click the **"Authorize"** button (🔒) in the top-right
5. Enter: `Bearer your_access_token`
6. Test any protected endpoint directly from the UI

## 📊 Database Schema

The application uses Entity Framework Core with auto-migrations. The database schema includes:

### Core Tables
- **AspNetUsers** - User accounts (extends Identity with FirstName/LastName)
- **Exercises** - Exercise definitions with user ownership
- **Sessions** - Workout sessions with date and calculated totals
- **Sets** - Individual sets with reps and weight
- **RefreshTokens** - JWT refresh tokens with rotation support

### Relationships
- **User → Exercises** (One-to-Many)
- **Exercise → Sessions** (One-to-Many, Cascade Delete)
- **Session → Sets** (One-to-Many, Cascade Delete)

## 🔐 Authentication Flow

### Registration/Login
```http
POST /api/auth/register
{
  "firstName": "John",
  "lastName": "Doe", 
  "email": "john@example.com",
  "password": "SecurePassword123!"
}

POST /api/auth/login
{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

Both endpoints return:
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "base64-encoded-token",
  "user": {
    "id": "user-guid",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "createdAt": "2025-10-19T18:01:49Z"
  }
}
```

### Token Refresh
```http
POST /api/auth/refresh
{
  "refreshToken": "your-refresh-token"
}
```

### Using Protected Endpoints
Include the access token in the Authorization header:
```http
Authorization: Bearer your-access-token
```

## 📋 API Endpoints

> **Note**: All endpoints except authentication require a valid JWT token in the Authorization header.

### 🔐 Authentication
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user account | ❌ |
| POST | `/api/auth/login` | User login | ❌ |
| POST | `/api/auth/refresh` | Refresh access token | ❌ |
| POST | `/api/auth/logout` | Logout and revoke tokens | ✅ |

### 👤 User Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users/me` | Get current user profile | ✅ |
| PATCH | `/api/users/me` | Update user profile | ✅ |

### 🏋️ Exercise Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/exercises` | List user exercises (with search) | ✅ |
| POST | `/api/exercises` | Create new exercise | ✅ |
| GET | `/api/exercises/{exerciseId}` | Get exercise details | ✅ |
| PATCH | `/api/exercises/{exerciseId}` | Update exercise | ✅ |
| DELETE | `/api/exercises/{exerciseId}` | Delete exercise and all sessions | ✅ |
| GET | `/api/exercises/{exerciseId}/history` | Get exercise workout history | ✅ |

### 📅 Session Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/exercises/{exerciseId}/sessions` | Create workout session | ✅ |
| GET | `/api/exercises/{exerciseId}/sessions` | List exercise sessions | ✅ |
| GET | `/api/sessions/{sessionId}` | Get session details with sets | ✅ |
| DELETE | `/api/sessions/{sessionId}` | Delete session and all sets | ✅ |
| POST | `/api/sessions/{sessionId}/sets` | Add set to session | ✅ |

### 📊 Set Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| PATCH | `/api/sets/{setId}` | Update exercise set | ✅ |
| DELETE | `/api/sets/{setId}` | Delete exercise set | ✅ |

### 📈 Dashboard
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/dashboard/exercises` | Dashboard overview with recent activity | ✅ |

## 🧪 Testing

### Using Enhanced Swagger UI (Recommended)
1. **Start the application**: `dotnet run`
2. **Open Swagger**: Navigate to `http://localhost:5032/swagger`
3. **Register/Login**: Use `/api/auth/register` or `/api/auth/login` endpoints
4. **Authorize**: Click the "Authorize" button and enter `Bearer your_access_token`
5. **Test Endpoints**: All protected endpoints will now work with your token
6. **Explore**: Browse organized endpoint categories and detailed documentation

### Using the HTTP File
The project includes a comprehensive test file (`SimplyTrack-API.http`) with 26+ test scenarios:

1. Open `SimplyTrack-API.http` in VS Code with REST Client extension
2. Start with user registration
3. Copy tokens from responses to test protected endpoints
4. Follow the complete workflow from registration to cleanup

### API Testing Features
- **🔒 JWT Authentication** - Integrated into Swagger UI
- **📝 Request Examples** - Sample payloads for all endpoints
- **📊 Response Documentation** - Expected response formats and status codes
- **🏷️ Organized Categories** - Grouped by functional areas
- **🔍 Search & Filter** - Easy endpoint discovery

### Sample Workout Flow
```http
# 1. Register/Login to get tokens
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "password"
}

# 2. Create an exercise
POST /api/exercises
Authorization: Bearer {access_token}
{
  "name": "Bench Press",
  "notes": "Chest exercise"
}

# 3. Create a session for today
POST /api/exercises/{exercise_id}/sessions
Authorization: Bearer {access_token}
{
  "date": "2025-10-19"
}

# 4. Add sets to the session
POST /api/sessions/{session_id}/sets
Authorization: Bearer {access_token}
{
  "reps": 10,
  "weight": 80.0
}

# 5. View dashboard
GET /api/dashboard/exercises
Authorization: Bearer {access_token}
```

## 📖 Documentation

### API Documentation
- **[API_DOCUMENTATION.md](API_DOCUMENTATION.md)** - Comprehensive API documentation with examples
- **Swagger UI** - Interactive documentation at `http://localhost:5032/swagger`
- **OpenAPI Specification** - Available in `openapi.yml`

### Key Documentation Features
- **Complete endpoint reference** with request/response examples
- **Authentication flow** documentation with JWT implementation details
- **Data model specifications** and relationship diagrams
- **Configuration guide** with environment variable reference
- **Testing workflows** and integration examples

## ⚙️ Configuration

### Environment Variables
| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `DB_HOST` | Database host | localhost | No |
| `DB_PORT` | Database port | 3306 | No |
| `DB_NAME` | Database name | simplytrack | No |
| `DB_USER` | Database user | root | No |
| `DB_PASS` | Database password | Your_password | No |
| `JWT_KEY` | JWT signing key | - | **Yes** |
| `JWT_ISSUER` | JWT issuer | SimplyTrack | No |
| `JWT_AUDIENCE` | JWT audience | SimplyTrackClient | No |
| `ACCESS_TOKEN_EXPIRATION_MINUTES` | Access token lifetime | 15 | No |
| `REFRESH_TOKEN_DAYS` | Refresh token lifetime | 30 | No |

### Configuration Priority
1. **Environment variables** (highest priority)
2. **appsettings.json** values
3. **Hardcoded defaults** (fallback)

## 🏗️ Architecture

### Project Structure
```
SimplyTrack-API/
├── Controllers/           # API controllers
├── Data/                  # Entity Framework DbContext
├── Models/                # Domain models
│   └── DTOs/             # Data Transfer Objects
├── Services/              # Business logic services
├── Migrations/            # EF Core migrations
├── Properties/            # Launch configuration
├── .env                   # Environment variables
├── appsettings.json       # Configuration
├── openapi.yml           # OpenAPI specification
└── SimplyTrack-API.http  # API tests
```

### Key Patterns
- **Environment-first Configuration** - Env vars override config files
- **Repository Pattern** - Data access abstraction
- **Service Layer** - Business logic separation
- **DTO Pattern** - Request/response validation
- **Auto-migration** - Database schema updates on startup

## 🛠️ Technologies

### Backend Stack
- **ASP.NET Core 8** - Web API framework
- **Entity Framework Core** - Object-relational mapping (ORM)
- **Pomelo MySQL Provider** - MySQL/MariaDB database provider
- **ASP.NET Core Identity** - User management and authentication
- **JWT Bearer Authentication** - Secure token-based authentication

### Development Tools
- **Swagger/OpenAPI** - API documentation and testing
- **DotNetEnv** - Environment variable management
- **Entity Framework Migrations** - Database schema management

### Database
- **MySQL/MariaDB** - Primary database with auto-server detection
- **Refresh Token Storage** - Persistent token management
- **Cascade Deletes** - Automatic cleanup of related data

### Architecture Features
- **RESTful API Design** - Following REST conventions
- **JWT with Refresh Tokens** - Secure authentication pattern
- **User-scoped Data** - Multi-tenant data isolation
- **Comprehensive Validation** - Input/output data validation
- **Auto-migrations** - Zero-downtime database updates

### Namespaces
- **Assembly**: `SimplyTrackAPI`
- **Root Namespace**: `SimplyTrack.Api`
- **DbContext Namespace**: `ApplicationDBContext.Api.Data`

## 🔧 Development

### Adding New Features
1. Create domain models in `/Models`
2. Add DTOs in `/Models/DTOs`
3. Implement services in `/Services`
4. Create controllers in `/Controllers`
5. Register services in `Program.cs`
6. Add tests to `.http` file

### Database Changes
```bash
# Create migration
dotnet ef migrations add YourMigrationName

# Apply migration
dotnet ef database update
```

### Running in Development
```bash
dotnet run --environment Development
```

## 📱 Mobile Integration

This API is designed for mobile applications (React Native, Flutter, etc.):

- **🔐 Token-based authentication** with refresh token rotation
- **📱 RESTful endpoints** following mobile-friendly patterns
- **🛡️ Comprehensive error handling** with structured error responses
- **📊 Rich data models** with nested relationships
- **🔄 Real-time updates** preparation for future WebSocket support
- **📈 Dashboard endpoints** optimized for mobile overview screens

### Integration Tips
- **Security**: Store refresh tokens securely (Keychain/Keystore)
- **Auto-refresh**: Implement automatic token refresh on 401 responses
- **UI Optimization**: Use dashboard endpoints for main app screens
- **Error Handling**: Implement proper error handling with the structured error responses
- **Performance**: Leverage search and pagination parameters
- **Testing**: Use Swagger UI for rapid prototyping and testing

### Mobile-Friendly Features
- **Hierarchical data structure** (User → Exercise → Session → Set)
- **User-scoped data isolation** for multi-user applications
- **Comprehensive validation** preventing invalid data states
- **Optimized payloads** with only necessary data
- **RESTful design** following mobile development best practices

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋 Support

For support, open an issue on GitHub.

---

**Built by Alvaro Mora with ❤️ for fitness enthusiasts and developers.**