# SimplyTrack API Documentation

## Overview
SimplyTrack is a comprehensive fitness tracking API built with ASP.NET Core 8, featuring JWT authentication and complete CRUD operations for managing exercises, workout sessions, and individual sets.

## API Features

### 🔐 Authentication & Authorization
- **JWT Bearer Token Authentication** - Secure API access with rotating refresh tokens
- **User Registration & Login** - Complete user management with ASP.NET Core Identity
- **Token Refresh** - Automatic token renewal for seamless user experience
- **Secure Logout** - Token revocation and cleanup

### 📊 Data Management
- **Exercise Management** - Create, read, update, and delete fitness exercises
- **Session Tracking** - Manage workout sessions with date tracking
- **Set Recording** - Track individual exercise sets with reps, weight, and order
- **Dashboard Overview** - Summary views with recent activity

### 🛡️ Security Features
- **User-scoped Data** - All data is isolated per authenticated user
- **Input Validation** - Comprehensive model validation on all endpoints
- **Error Handling** - Consistent error responses with proper HTTP status codes

## Enhanced Swagger Documentation

### 🎯 New Features Added:
1. **JWT Authentication Support**
   - "Authorize" button in Swagger UI
   - Bearer token configuration
   - Secure endpoint testing

2. **Comprehensive API Documentation**
   - Detailed endpoint descriptions
   - Parameter documentation
   - Response type specifications
   - HTTP status code documentation

3. **Organized API Structure**
   - Categorized by functional areas:
     - Authentication
     - User Management
     - Exercise Management
     - Session Management
     - Set Management
     - Dashboard

4. **Enhanced Metadata**
   - API version information
   - Contact details
   - License information
   - Detailed descriptions

## API Endpoints Summary

### Authentication Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user account | ❌ |
| POST | `/api/auth/login` | Authenticate user | ❌ |
| POST | `/api/auth/refresh` | Refresh access token | ❌ |
| POST | `/api/auth/logout` | Logout and revoke tokens | ✅ |

### User Management Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users/me` | Get current user profile | ✅ |
| PATCH | `/api/users/me` | Update current user profile | ✅ |

### Exercise Management Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/exercises` | Get all user exercises | ✅ |
| POST | `/api/exercises` | Create new exercise | ✅ |
| GET | `/api/exercises/{exerciseId}` | Get exercise details | ✅ |
| PATCH | `/api/exercises/{exerciseId}` | Update exercise | ✅ |
| DELETE | `/api/exercises/{exerciseId}` | Delete exercise | ✅ |
| GET | `/api/exercises/{exerciseId}/history` | Get exercise workout history | ✅ |

### Session Management Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/exercises/{exerciseId}/sessions` | Create workout session | ✅ |
| GET | `/api/exercises/{exerciseId}/sessions` | Get exercise sessions | ✅ |
| GET | `/api/sessions/{sessionId}` | Get session details | ✅ |
| DELETE | `/api/sessions/{sessionId}` | Delete session | ✅ |
| POST | `/api/sessions/{sessionId}/sets` | Add set to session | ✅ |

### Set Management Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| PATCH | `/api/sets/{setId}` | Update exercise set | ✅ |
| DELETE | `/api/sets/{setId}` | Delete exercise set | ✅ |

### Dashboard Endpoints
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/dashboard/exercises` | Get dashboard exercise overview | ✅ |

## Authentication Flow

### 1. Registration/Login
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

### 2. Using Protected Endpoints
```http
GET /api/users/me
Authorization: Bearer your_jwt_token_here
```

### 3. Token Refresh
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "your_refresh_token_here"
}
```

## Response Types

### Success Responses
- **200 OK** - Successful operation
- **201 Created** - Resource created successfully
- **204 No Content** - Successful deletion

### Error Responses
- **400 Bad Request** - Invalid input data
- **401 Unauthorized** - Authentication required or invalid
- **404 Not Found** - Resource not found
- **422 Unprocessable Entity** - Validation errors

## Data Models

### Core Entities
- **Exercise** - Fitness exercise definition
- **Session** - Individual workout session
- **SetEntity** - Individual exercise set with reps/weight
- **ApplicationUser** - User account with profile information

### Hierarchical Structure
```
User
└── Exercise
    └── Session
        └── Set (SetEntity)
```

## Development Setup

### Prerequisites
- .NET 8.0 SDK
- MySQL/MariaDB database
- Environment variables or appsettings.json configuration

### Running the API
```bash
dotnet run
```

### Accessing Swagger UI
Navigate to: `http://localhost:5032/swagger`

### Authentication in Swagger
1. Use `/api/auth/register` or `/api/auth/login` to get tokens
2. Click "Authorize" button in Swagger UI
3. Enter: `Bearer your_access_token`
4. Test protected endpoints

## Configuration

### Environment Variables
- `JWT_KEY` - JWT signing key (required, min 32 chars)
- `JWT_ISSUER` - Token issuer
- `JWT_AUDIENCE` - Token audience
- `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASS` - Database connection

### Features
- Auto-migration on startup
- Refresh token rotation
- User-scoped data isolation
- Comprehensive input validation
- Detailed error responses

---

*Generated on: October 19, 2025*
*API Version: v1*