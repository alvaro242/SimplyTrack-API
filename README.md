# SimplyTrack API

**SimplyTrack API** is a backend application developed using .NET 8 Web API. The project is designed to serve as the core for the SimplyTrack system, enabling efficient data processing and interaction via RESTful endpoints. It uses MariaDB as its database and follows secure practices by leveraging environment variables for sensitive configuration details.

## Features
- Basic structure for RESTful API endpoints.
- Integration with MariaDB using Entity Framework Core with the Pomelo provider.
- Environment variable support for secure management of database credentials.
- Dockerized deployment for seamless scalability.

## Setup

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/)
- MariaDB server
- Docker (for containerization, optional)

### Configuration
1. Clone the repository:
   ```bash
   git clone https://github.com/alvaro242/SimplyTrack-API.git
   cd SimplyTrack-API

2. Create a .env file: In the root of the project, create a .env file with the following content:
   ```bash
    DB_SERVER=localhost
    DB_DATABASE=SimplyTrack
    DB_USER=YourDatabaseUser
    DB_PASSWORD=YourDatabasePassword

3. Add .env to .gitignore: To prevent sensitive information from being committed:
    ```bash
      .env

### Database Migration
  1. Install Entity Framework Core tools:
       ```bash
       dotnet tool install --global dotnet-ef
    
  2. Create an initial migration:
       ```bash
        dotnet ef migrations add InitialCreate
  3. Apply Migrations:
    ```bash
        dotnet ef database update

### Running Locally
  1. Restore dependencies:
      ```bash
        dotnet restore
  2. Run the application:
      ```bash
      dotnet run
  3. Access the API: Visit the Swagger documentation at:
     ```bash
      http://localhost:5000/swagger
### Docker Deployment

  1. Build the Docker image:
      ```bash
        docker build -t simplytrack-api .
  2. Run the Docker container:
      ```bash
        docker run -d -p 8060:80 --env-file .env simplytrack-api
  3. Access the API: Visit the Swagger documentation at:
       ```bash
      http://localhost:8060/swagger

## Technologies Used
- .NET 8: For building the backend.

- MariaDB: Database management.

- Entity Framework Core: Object-relational mapping (ORM).

- Pomelo.EntityFrameworkCore.MySql: MariaDB provider for EF Core.

- Docker: For containerized deployment.

       





# SimplyTrack API

A comprehensive fitness tracking REST API built with ASP.NET Core 8, featuring JWT authentication, refresh tokens, and complete workout management capabilities. Perfect for mobile apps and fitness tracking applications.

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

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/refresh` | Refresh access token |
| POST | `/api/auth/logout` | Logout (revoke refresh token) |
| POST | `/api/auth/revoke` | Revoke specific refresh token |

### User Management
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/me` | Get current user profile |
| PATCH | `/api/users/me` | Update user profile |
| DELETE | `/api/users/me` | Delete user account |

### Exercises
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/exercises` | List user exercises |
| POST | `/api/exercises` | Create new exercise |
| GET | `/api/exercises/{id}` | Get exercise details |
| PATCH | `/api/exercises/{id}` | Update exercise |
| DELETE | `/api/exercises/{id}` | Delete exercise |
| GET | `/api/exercises/{id}/history` | Get exercise history |

### Sessions
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/exercises/{id}/sessions` | Create workout session |
| GET | `/api/exercises/{id}/sessions` | List exercise sessions |
| GET | `/api/sessions/{id}` | Get session with sets |
| DELETE | `/api/sessions/{id}` | Delete session |

### Sets
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/sessions/{id}/sets` | Add set to session |
| PATCH | `/api/sets/{id}` | Update set |
| DELETE | `/api/sets/{id}` | Delete set |

### Dashboard
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/dashboard/exercises` | Dashboard with last session summaries |

## 🧪 Testing

### Using the HTTP File
The project includes a comprehensive test file (`SimplyTrack-API.http`) with 26+ test scenarios:

1. Open `SimplyTrack-API.http` in VS Code with REST Client extension
2. Start with user registration
3. Copy tokens from responses to test protected endpoints
4. Follow the complete workflow from registration to cleanup

### Using Swagger UI
Visit `http://localhost:5032/swagger` in development mode for interactive API documentation.

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

- **Token-based authentication** with refresh token rotation
- **RESTful endpoints** following mobile-friendly patterns
- **Comprehensive error handling** with structured error responses
- **Offline sync support** preparation (clientId fields for deduplication)

### Integration Tips
- Store refresh tokens securely (Keychain/Keystore)
- Implement automatic token refresh on 401 responses
- Use the dashboard endpoint for main app screens
- Implement optimistic UI updates with proper error handling

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