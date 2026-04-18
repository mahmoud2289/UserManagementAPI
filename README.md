# UserManagementAPI

A RESTful API built with **ASP.NET Core 10 Minimal APIs** for managing users. It supports full CRUD operations, token-based authentication, input validation, and structured middleware for logging and error handling.

---

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Clone the Repository](#clone-the-repository)
  - [Build the Project](#build-the-project)
  - [Run the Application](#run-the-application)
- [Configuration](#configuration)
- [Authentication](#authentication)
- [API Reference](#api-reference)
  - [Health Check](#health-check)
  - [Get All Users](#get-all-users)
  - [Get User by ID](#get-user-by-id)
  - [Create User](#create-user)
  - [Update User](#update-user)
  - [Delete User](#delete-user)
- [Data Model](#data-model)
- [Validation Rules](#validation-rules)
- [Middleware Pipeline](#middleware-pipeline)
- [Error Handling](#error-handling)
- [Testing](#testing)
- [Project Structure](#project-structure)

---

## Features

- Full CRUD operations for user management
- Token-based authentication (Bearer token)
- Input validation with descriptive error messages
- Structured middleware pipeline (error handling → authentication → logging)
- Swagger/OpenAPI documentation (available in development)
- In-memory data store with three seed users
- Health check endpoint

---

## Technology Stack

| Component | Technology |
|-----------|-----------|
| Framework | ASP.NET Core 10.0 (Minimal APIs) |
| Language | C# |
| API Documentation | Swagger (Swashbuckle.AspNetCore 10.1.4) |
| OpenAPI | Microsoft.AspNetCore.OpenApi 10.0.1 |
| Logging | ASP.NET Core built-in console logging |
| Data storage | In-memory (no database) |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A REST client for testing, such as:
  - [VS Code REST Client extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)
  - [Postman](https://www.postman.com/)
  - `curl`

---

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/mahmoud2289/UserManagementAPI.git
cd UserManagementAPI
```

### Build the Project

```bash
dotnet build
```

### Run the Application

**HTTP (port 5019):**

```bash
dotnet run --launch-profile http
```

**HTTPS (port 7158):**

```bash
dotnet run --launch-profile https
```

Once running, the API is available at:

- HTTP: `http://localhost:5019`
- HTTPS: `https://localhost:7158`
- Swagger UI: `http://localhost:5019/swagger` (Development mode only)

---

## Configuration

### `appsettings.json`

Controls default logging behaviour:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### `appsettings.Development.json`

Overrides for the development environment.

---

## Authentication

All `/api/users` endpoints require a **Bearer token** in the `Authorization` header.

```
Authorization: Bearer <token>
```

### Valid Tokens (Demo/Test Only)

| Token | Description |
|-------|-------------|
| `token-12345` | Simple demo token |
| `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9` | JWT-style demo token |
| `demo-token-xyz` | Alternative demo token |

> ⚠️ **Note:** These are hardcoded tokens for demonstration purposes only. A production deployment must replace this with a proper JWT or OAuth 2.0 implementation.

### Exempt Endpoints

The following paths bypass authentication entirely:

- `GET /health`
- `GET /openapi/*`
- `GET /swagger/*`

### Unauthorized Response (401)

```json
{
  "status": 401,
  "message": "Unauthorized",
  "detail": "Missing or invalid authorization token. Please provide a valid Bearer token."
}
```

---

## API Reference

All endpoints (except `/health`) require the `Authorization` header described above.

**Base URL:** `http://localhost:5019`

---

### Health Check

Check whether the service is running. No authentication required.

```
GET /health
```

**Response — 200 OK**

```json
{
  "status": "healthy"
}
```

---

### Get All Users

Retrieve a list of all users, ordered by ID.

```
GET /api/users
Authorization: Bearer token-12345
```

**Response — 200 OK**

```json
[
  {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@techhive.com",
    "phoneNumber": "555-0101",
    "department": "Engineering",
    "createdAt": "2026-01-21T13:04:39.840Z",
    "updatedAt": "2026-01-21T13:04:39.840Z"
  },
  {
    "id": 2,
    "firstName": "Jane",
    "lastName": "Smith",
    "email": "jane.smith@techhive.com",
    "phoneNumber": "555-0102",
    "department": "Human Resources",
    "createdAt": "2026-01-31T13:04:39.840Z",
    "updatedAt": "2026-01-31T13:04:39.840Z"
  }
]
```

---

### Get User by ID

Retrieve a single user by their numeric ID.

```
GET /api/users/{id}
Authorization: Bearer token-12345
```

**Path Parameters**

| Parameter | Type | Description |
|-----------|------|-------------|
| `id` | integer | The user's unique ID (must be greater than 0) |

**Responses**

| Status | Description |
|--------|-------------|
| 200 OK | User found and returned |
| 400 Bad Request | `id` is ≤ 0 |
| 404 Not Found | No user with the given ID exists |

**Response — 200 OK**

```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@techhive.com",
  "phoneNumber": "555-0101",
  "department": "Engineering",
  "createdAt": "2026-01-21T13:04:39.840Z",
  "updatedAt": "2026-01-21T13:04:39.840Z"
}
```

**Response — 404 Not Found**

```json
{
  "message": "User with ID 999 not found"
}
```

**Response — 400 Bad Request**

```json
{
  "message": "Invalid user ID"
}
```

---

### Create User

Create a new user. The `id`, `createdAt`, and `updatedAt` fields are set automatically by the server.

```
POST /api/users
Authorization: Bearer token-12345
Content-Type: application/json
```

**Request Body**

```json
{
  "firstName": "Alice",
  "lastName": "Anderson",
  "email": "alice.anderson@techhive.com",
  "phoneNumber": "555-0104",
  "department": "Finance"
}
```

**Responses**

| Status | Description |
|--------|-------------|
| 201 Created | User created successfully |
| 400 Bad Request | Validation failed or email already exists |
| 401 Unauthorized | Missing or invalid token |

**Response — 201 Created**

```json
{
  "id": 4,
  "firstName": "Alice",
  "lastName": "Anderson",
  "email": "alice.anderson@techhive.com",
  "phoneNumber": "555-0104",
  "department": "Finance",
  "createdAt": "2026-02-20T13:04:39.840Z",
  "updatedAt": "2026-02-20T13:04:39.840Z"
}
```

**Response — 400 Bad Request (validation error)**

```json
{
  "message": "Email format is invalid"
}
```

**Response — 400 Bad Request (duplicate email)**

```json
{
  "message": "A user with email 'alice.anderson@techhive.com' already exists."
}
```

---

### Update User

Replace all fields of an existing user. The `updatedAt` timestamp is set automatically.

```
PUT /api/users/{id}
Authorization: Bearer token-12345
Content-Type: application/json
```

**Path Parameters**

| Parameter | Type | Description |
|-----------|------|-------------|
| `id` | integer | The user's unique ID (must be greater than 0) |

**Request Body**

```json
{
  "firstName": "John",
  "lastName": "Updated",
  "email": "john.updated@techhive.com",
  "phoneNumber": "555-9999",
  "department": "IT"
}
```

**Responses**

| Status | Description |
|--------|-------------|
| 200 OK | User updated successfully |
| 400 Bad Request | Invalid ID or validation failed |
| 404 Not Found | No user with the given ID exists |

**Response — 200 OK**

```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Updated",
  "email": "john.updated@techhive.com",
  "phoneNumber": "555-9999",
  "department": "IT",
  "createdAt": "2026-01-21T13:04:39.840Z",
  "updatedAt": "2026-02-20T13:04:39.840Z"
}
```

---

### Delete User

Remove a user by ID.

```
DELETE /api/users/{id}
Authorization: Bearer token-12345
```

**Path Parameters**

| Parameter | Type | Description |
|-----------|------|-------------|
| `id` | integer | The user's unique ID (must be greater than 0) |

**Responses**

| Status | Description |
|--------|-------------|
| 204 No Content | User deleted successfully |
| 400 Bad Request | `id` is ≤ 0 |
| 404 Not Found | No user with the given ID exists |

---

## Data Model

### User

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `id` | integer | Auto | Unique identifier (assigned by server) |
| `firstName` | string | ✅ | User's first name |
| `lastName` | string | ✅ | User's last name |
| `email` | string | ✅ | Unique email address |
| `phoneNumber` | string | ✅ | Contact phone number |
| `department` | string | ✅ | User's department |
| `createdAt` | DateTime (UTC) | Auto | Record creation timestamp (assigned by server) |
| `updatedAt` | DateTime (UTC) | Auto | Last update timestamp (assigned by server) |

---

## Validation Rules

| Field | Rules |
|-------|-------|
| `firstName` | Required; max 100 characters |
| `lastName` | Required; max 100 characters |
| `email` | Required; valid email format (`user@domain.tld`); max 255 characters; must be unique (case-insensitive) |
| `phoneNumber` | Required; must match one of the accepted formats (see below); max 20 characters |
| `department` | Required; max 100 characters |

### Accepted Phone Number Formats

| Format | Example |
|--------|---------|
| `NXX-XXXX` (7-digit local) | `555-0101` |
| 10-digit numeric | `5550101234` |
| International (`+` prefix or with spaces/dashes/parentheses, ≥ 10 digits) | `+1-234-567-8900` |

---

## Middleware Pipeline

The middleware pipeline executes in the following order on every request:

```
Request → ErrorHandlingMiddleware → AuthenticationMiddleware → LoggingMiddleware → Endpoint Handler → Response
```

### 1. ErrorHandlingMiddleware

- Wraps the entire pipeline in a `try/catch`.
- Returns a `500 Internal Server Error` JSON response for any unhandled exceptions.
- Logs the error message via the ASP.NET Core logger.

### 2. AuthenticationMiddleware

- Reads the `Authorization` header.
- Allows requests to `/health`, `/openapi`, and `/swagger` to pass through without a token.
- Returns `401 Unauthorized` if the header is absent or the token is invalid.
- Logs the authentication outcome.

### 3. LoggingMiddleware

- Logs each incoming request as `[REQUEST] METHOD /path`.
- Logs each outgoing response as `[RESPONSE] METHOD /path - Status: XXX`.
- Uses a memory stream buffer so the response body is available for capture before being written to the client.

---

## Error Handling

All error responses follow a consistent JSON structure:

### Validation / Business Logic Error (400)

```json
{
  "message": "<descriptive error message>"
}
```

### Unauthorized (401)

```json
{
  "status": 401,
  "message": "Unauthorized",
  "detail": "Missing or invalid authorization token. Please provide a valid Bearer token."
}
```

### Not Found (404)

```json
{
  "message": "User with ID <id> not found"
}
```

### Internal Server Error (500)

```json
{
  "status": 500,
  "message": "An error occurred while processing your request.",
  "detail": "<exception message>"
}
```

---

## Testing

The repository includes two HTTP test files compatible with the **VS Code REST Client** extension.

### `test_api.http`

Contains **19 test cases** covering:

| Test # | Scenario | Expected Result |
|--------|----------|-----------------|
| 1 | GET all users | 200 OK |
| 2 | GET user by valid ID | 200 OK |
| 3 | GET user by non-existent ID | 404 Not Found |
| 4 | GET user by negative ID | 400 Bad Request |
| 5 | POST valid user | 201 Created |
| 6 | POST with missing first name | 400 Bad Request |
| 7 | POST with missing last name | 400 Bad Request |
| 8 | POST with invalid email format | 400 Bad Request |
| 9 | POST with missing email | 400 Bad Request |
| 10 | POST with invalid phone format | 400 Bad Request |
| 11 | POST with missing department | 400 Bad Request |
| 12 | POST with duplicate email | 400 Bad Request |
| 13 | PUT valid update | 200 OK |
| 14 | PUT with non-existent ID | 404 Not Found |
| 15 | PUT with negative ID | 400 Bad Request |
| 16 | PUT with missing email | 400 Bad Request |
| 17 | DELETE valid user | 204 No Content |
| 18 | DELETE non-existent user | 404 Not Found |
| 19 | DELETE with negative ID | 400 Bad Request |

### `UserManagementAPI.http`

Sample request file for quick manual testing.

### Running Tests with VS Code REST Client

1. Install the [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension.
2. Open `test_api.http`.
3. Start the API with `dotnet run --launch-profile http`.
4. Click **Send Request** above any test block.

### Swagger UI

In development, an interactive API browser is available at:

```
http://localhost:5019/swagger
```

---

## Project Structure

```
UserManagementAPI/
├── Middleware/
│   ├── AuthenticationMiddleware.cs   # Bearer token validation
│   ├── ErrorHandlingMiddleware.cs    # Global exception handler
│   └── LoggingMiddleware.cs          # Request/response logger
├── Models/
│   └── User.cs                       # User data model
├── Services/
│   └── UserService.cs                # Business logic & in-memory data store
├── Validators/
│   └── UserValidator.cs              # Input validation logic
├── Properties/
│   └── launchSettings.json           # Development server profiles
├── Program.cs                        # Application entry point & route definitions
├── UserManagementAPI.csproj          # Project file & NuGet dependencies
├── appsettings.json                  # Production logging configuration
├── appsettings.Development.json      # Development logging overrides
├── test_api.http                     # 19 REST Client test cases
└── UserManagementAPI.http            # Sample REST Client requests
```

---

## Seed Data

On startup the in-memory store is pre-populated with three users:

| ID | Name | Email | Department |
|----|------|-------|------------|
| 1 | John Doe | john.doe@techhive.com | Engineering |
| 2 | Jane Smith | jane.smith@techhive.com | Human Resources |
| 3 | Mike Johnson | mike.johnson@techhive.com | IT |

> **Note:** All data is stored in memory and is reset every time the application restarts.
