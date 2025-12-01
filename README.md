# Authentication API (.NET 6)

A robust authentication API built with .NET 6 following Clean Architecture principles. This API provides user registration, login, JWT token management, and refresh token functionality.

## 🏗️ Architecture

This project follows Clean Architecture with the following layers:

- **AUTH.API** - Web API layer containing controllers and configuration
- **SERVICES** - Business logic and application services
- **CORE** - Domain entities, DTOs, and interfaces
- **REPOSITORY** - Data access layer with Entity Framework Core

## 🚀 Features

- User registration and login
- JWT access token generation
- Refresh token mechanism
- Password hashing with PBKDF2
- Email uniqueness validation
- Secure logout functionality
- Swagger documentation

## 📋 Prerequisites

- .NET 6 SDK
- SQL Server (or SQL Server LocalDB)
- Visual Studio 2022 or VS Code

## ⚙️ Setup & Installation

1. **Clone the repository**```bash
git clone https://github.com/yourusername/authentication-api.git
cd authentication-api
```
2. **Configure Database Connection**

Update the connection string in `AUTH.API/appsettings.json`:```json
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuthDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
3. **Configure JWT Settings**

Add JWT configuration to `AUTH.API/appsettings.json`:```json
"JwtSettings": {
    "Secret": "YourSuperSecretKey",
    "Issuer": "YourIssuer",
    "Audience": "YourAudience",
    "AccessTokenExpiration": 30,
    "RefreshTokenExpiration": 7
    }
```
4. **Run Database Migrations**```bash
dotnet ef database update --project REPOSITORY
```
5. **Run the Application**```bash
dotnet run --project AUTH.API
```
The API will be available at `https://localhost:7xxx` with Swagger documentation.

## 📡 API Endpoints

### Authentication Controller (`/Auth`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/Auth/Register` | Register a new user |
| POST | `/Auth/Login` | User login |
| POST | `/Auth/RefreshToken` | Refresh access token |
| POST | `/Auth/LogOut` | Logout user |
| GET | `/Auth/Test` | Test protected endpoint |

## 📝 Usage Examples

### User Registration```http
POST /Auth/Register
Content-Type: application/json

{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "password": "YourPassword123!"
    }
```
### User Login```http
POST /Auth/Login
Content-Type: application/json

{
    "email": "john.doe@example.com",
    "password": "YourPassword123!"
    }
```
**Response:**
```json
{
    "accessToken": "your-jwt-access-token",
    "refreshToken": "your-refresh-token"
    }
```
### Refresh Token```http
POST /Auth/RefreshToken
Content-Type: application/json

{
    "accessToken": "your-jwt-access-token",
    "refreshToken": "your-refresh-token"
    }
```
### Logout```http
POST /Auth/LogOut
Content-Type: application/json

{
    "refreshToken": "your-refresh-token"
}
```
### Test Protected Endpoint```http
GET /Auth/Test
Authorization: Bearer your-jwt-access-token
```
## 🔐 Security Features

- **Password Hashing**: Uses PBKDF2 with SHA512 and salt (10,000 iterations)
- **JWT Tokens**: Secure access token with configurable expiration
- **Refresh Tokens**: 7-day expiration with secure regeneration
- **Email Validation**: Ensures unique email addresses
- **Input Validation**: Comprehensive DTO validation
- **Claims-based Authentication**: Includes user ID, email, and verification status

## 🗃️ Database Schema

### Users Table
- `Id` (string, PK)
- `FirstName` (string, 100 chars)
- `LastName` (string, 100 chars)
- `Email` (string, 255 chars, unique)
- `HashPassword` (string)
- `IsVerified` (bool, default: false)
- `CreatedAt` (DateTimeOffset)

### RefreshTokens Table
- `Id` (string, PK)
- `Token` (string)
- `ExpiresAt` (DateTimeOffset)
- `CreatedAt` (DateTimeOffset)
- `UserId` (string, FK)

## 🛠️ Development

### Project Structure```plaintext
├── AUTH.API
├── SERVICES
├── CORE
└── REPOSITORY
```
### Adding New Features
1. Define interfaces in the `CORE` project
2. Implement business logic in the `SERVICES` project
3. Add data access methods in the `REPOSITORY` project
4. Create controllers in the `AUTH.API` project

### Running Tests```bash
dotnet test
```
## 📦 Dependencies

### Main Packages
- **Microsoft.EntityFrameworkCore** - Data access
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication
- **AutoMapper** - Object mapping
- **Swashbuckle.AspNetCore** - API documentation
- **Microsoft.EntityFrameworkCore.Tools** - EF migrations

### JWT Claims Structure
- `NameIdentifier`: User ID
- `Email`: User email address
- `EmailVerify`: Email verification status

## 🔧 Configuration

### Required Configuration Sections

#### Database```json
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuthDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
```
#### JWT```json
"JwtSettings": {
    "Secret": "YourSuperSecretKey",
    "Issuer": "YourIssuer",
    "Audience": "YourAudience",
    "AccessTokenExpiration": 30,
    "RefreshTokenExpiration": 7
    }
```
## 🚨 Error Responses

Common error response format:```json
{
    "errors": [
        {
            "code": "Error Code",
            "message": "Error message"
        }
    ]
}
```
## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is licensed under the MIT License.
