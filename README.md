# TalentoPlus S.A.S. - Employee Management System

## Project Overview
TalentoPlus S.A.S. is modernizing its Human Resources department by replacing manual Excel-based processes with a comprehensive web application and REST API built on .NET 9. This system centralizes employee data management, automates workflows, and provides intelligent insights through AI integration.

## Business Problem Solved
The company previously managed employee information through Excel files circulated via email, leading to duplicated, outdated, or incomplete data. HR staff had to manually search multiple files to prepare employee documents. This system eliminates those inefficiencies by providing:
- **Centralized database** for all employee information
- **Automated processes** for employee management
- **Self-service portal** for employees
- **AI-powered analytics** for decision support

##  Architecture & Design
The solution follows Clean Architecture principles with clear separation of concerns:

### Project Structure
```
TalentoPlus/
├── TalentoPlus.Domain/          # Entities, Enums, Interfaces
├── TalentoPlus.Application/     # DTOs, Services, Business Logic
├── TalentoPlus.Infrastructure/  # Repositories, DbContext, External Services
├── TalentoPlus.Api/            # REST API (Employee Portal)
├── TalentoPlus.Web/            # MVC Web App (HR Admin)
└── docker-compose.yml          # Container orchestration
```

### Key Design Patterns
- **Repository Pattern**: Abstract data access layer
- **Dependency Injection**: Loose coupling between components
- **JWT Authentication**: Secure API access for employees
- **Session-based Authentication**: For admin web interface
- **Clean Architecture**: Clear separation between presentation, business logic, and data access

## Technologies Used

### Backend
- **.NET 9** - Primary framework
- **ASP.NET Core** - Web framework
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL** - Primary database (MySQL also supported)
- **AutoMapper** - Object-object mapping
- **BCrypt.Net** - Password hashing
- **JWT Bearer Tokens** - API authentication
- **QuestPDF** - PDF generation for employee profiles

### Frontend (Admin Panel)
- **ASP.NET Core MVC** - Web interface for HR administrators
- **Razor Views** - Server-side rendering
- **Bootstrap** - Responsive UI components
- **Session Management** - Admin authentication

### DevOps & Tools
- **Docker & Docker Compose** - Containerization
- **Swagger/OpenAPI** - API documentation
- **Postman** - API testing
- **Environment Variables** - Secure configuration management

## Prerequisites
- .NET 9 SDK
- Docker & Docker Compose
- PostgreSQL (or use Docker version)
- Visual Studio 2022+ or VS Code
- Modern web browser

## Quick Start Guide

### Option 1: Using Docker (Recommended)
```bash
# Clone the repository
git clone <repository-url>
cd TalentoPlus

# Start all services
docker-compose up -d

# Access the applications:
# - API: http://localhost:5000
# - Web Admin: http://localhost:5001
# - PostgreSQL: localhost:5432
```

### Option 2: Manual Setup
```bash
# 1. Clone the repository
git clone <repository-url>
cd TalentoPlus

# 2. Configure database connection
# Edit appsettings.json in TalentoPlus.Api and TalentoPlus.Web
# Update ConnectionStrings.DefaultConnection

# 3. Apply database migrations
cd TalentoPlus.Api
dotnet ef database update

# 4. Run the API
dotnet run

# 5. Run the Web Admin (new terminal)
cd ../TalentoPlus.Web
dotnet run
```

## Default Credentials

### Admin Access (Web Application)
- **URL**: http://localhost:5001
- **Email**: admin@talento.com
- **Password**: Admin123!

### API Access
- **Base URL**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

## API Endpoints Guide

### Public Endpoints (No Authentication Required)

#### 1. List Departments
```http
GET /api/departamentos
```
**Purpose**: Get all available departments for employee registration forms.

**Response**:
```json
[
  {
    "id": 1,
    "nombre": "Recursos Humanos",
    "descripcion": "Gestión de personal"
  },
  {
    "id": 2,
    "nombre": "Tecnología",
    "descripcion": "Desarrollo y soporte IT"
  }
]
```

#### 2. Employee Self-Registration
```http
POST /api/empleados/registro
Content-Type: application/json
```
**Request Body**:
```json
{
  "documento": "98765432",
  "nombres": "Juan",
  "apellidos": "Perez",
  "email": "juan.perez@company.com",
  "password": "SecurePass123!",
  "fechaNacimiento": "1990-05-15",
  "telefono": "3001234567",
  "departamentoId": 2
}
```

**Response**: 201 Created with employee details

#### 3. Employee Login
```http
POST /api/auth/login
Content-Type: application/json
```
**Request Body**:
```json
{
  "documento": "98765432",
  "password": "SecurePass123!"
}
```
OR
```json
{
  "email": "juan.perez@company.com",
  "password": "SecurePass123!"
}
```
**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expires": "2024-12-09T16:30:00Z",
  "empleado": {
    "id": 1,
    "documento": "98765432",
    "nombres": "Juan",
    "apellidos": "Perez",
    "email": "juan.perez@company.com"
  }
}
```

### Protected Endpoints (Require JWT Token)

#### 1. Get Employee Profile
```http
GET /api/empleados/me
Authorization: Bearer <jwt_token>
```
**Purpose**: Employees can view their own information.

#### 2. Download Employee Profile PDF
```http
GET /api/empleados/me/pdf
Authorization: Bearer <jwt_token>
```
**Purpose**: Generate and download a professional PDF profile.

### Admin Endpoints (Require Admin Role)

#### 1. Dashboard Statistics
```http
GET /api/dashboard/stats
Authorization: Bearer <admin_jwt_token>
```
**Response**:
```json
{
  "totalEmpleados": 105,
  "empleadosVacaciones": 12,
  "empleadosActivos": 85,
  "empleadosPorDepartamento": {
    "Tecnología": 45,
    "Recursos Humanos": 15,
    "Operaciones": 25,
    "Marketing": 12,
    "Logística": 8
  }
}
```

#### 2. AI-Powered Queries
```http
POST /api/dashboard/query
Authorization: Bearer <admin_jwt_token>
Content-Type: application/json
```
**Request Body**:
```json
{
  "pregunta": "¿Cuántos auxiliares hay en la plataforma?"
}
```

## Testing with Swagger

### Step-by-Step Guide:

1. **Access Swagger UI**
    - Open your browser and navigate to: `http://localhost:5000/swagger`

2. **Test Public Endpoints**
    - Click on `GET /api/departamentos`
    - Click "Try it out" → "Execute"
    - Verify you receive the list of departments

3. **Register a New Employee**
    - Click on `POST /api/empleados/registro`
    - Click "Try it out"
    - Fill in the request body with employee data
    - Click "Execute"
    - Note the employee ID in the response

4. **Login as Employee**
    - Click on `POST /api/auth/login`
    - Use the credentials from registration
    - Copy the JWT token from the response

5. **Test Protected Endpoints**
    - Click the "Authorize" button at the top
    - Enter: `Bearer <your_jwt_token>`
    - Now test `GET /api/empleados/me`

## Testing with Postman

### Collection Setup:

1. **Import Environment Variables**
   ```json
   {
     "base_url": "http://localhost:5000",
     "admin_token": "",
     "employee_token": ""
   }
   ```

2. **Create Request Collection**:

   **Request 1: Get Departments**
   ```
   GET {{base_url}}/api/departamentos
   ```

   **Request 2: Employee Registration**
   ```
   POST {{base_url}}/api/empleados/registro
   Body (raw JSON): { ...employee data... }
   ```

   **Request 3: Employee Login**
   ```
   POST {{base_url}}/api/auth/login
   Body: { "documento": "...", "password": "..." }
   
   Tests Tab:
   const jsonData = pm.response.json();
   pm.environment.set("employee_token", jsonData.token);
   ```

   **Request 4: Get Employee Profile**
   ```
   GET {{base_url}}/api/empleados/me
   Headers: Authorization: Bearer {{employee_token}}
   ```


### Common Issues:

1. **Database Connection Failed**
   ```
   Solution: Verify PostgreSQL is running and connection string is correct
   Command: sudo service postgresql status
   ```

2. **Migration Errors**
   ```
   Solution: Delete migrations folder and recreate
   Command: dotnet ef migrations add InitialCreate
   Command: dotnet ef database update
   ```

3. **JWT Token Invalid**
   ```
   Solution: Check JWT key length (minimum 32 characters)
   Ensure token hasn't expired (default: 120 minutes)
   ```

4. **CORS Errors in Browser**
   ```
   Solution: Verify CORS policy in Program.cs
   Ensure frontend URL is in allowed origins
   ```

## Project Structure Details

### Domain Layer (`TalentoPlus.Domain/`)
- **Entities/**: `Empleado`, `Departamento`
- **Enums/**: `EstadoEmpleado`, `NivelEducativo`
- **Interfaces/**: `IRepository<T>`, `IEmpleadoRepository`

### Application Layer (`TalentoPlus.Application/`)
- **DTOs/**: Data transfer objects for API communication
- **Interfaces/**: Service contracts (`IEmpleadoService`, `IAuthService`)
- **Services/**: Business logic implementation
- **Mapping/**: AutoMapper profiles

### Infrastructure Layer (`TalentoPlus.Infrastructure/`)
- **Data/**: `AppDbContext`, entity configurations
- **Repositories/**: Database operations implementation
- **Services/**: External service integrations (Email, AI)

### API Layer (`TalentoPlus.Api/`)
- **Controllers/**: REST API endpoints
- **Middleware/**: Custom middleware (error handling, logging)
- **Program.cs**: Service configuration and startup
