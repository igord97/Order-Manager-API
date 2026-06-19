# OrderManager

OrderManager is an ASP.NET Core Web API learning project focused on production-style backend development and company-style layered architecture.

The project demonstrates ASP.NET Core Web API, SQL Server, Entity Framework Core, repository-based data access, service-layer business logic, API request/response models, custom middleware, global exception handling, JWT authentication, role-based authorization, password hashing, Swagger JWT bearer support, EF Core migrations, audit fields, and integration testing.

The architecture was recently refactored to better match the company convention used in existing projects:

```text
Core
  -> shared/common code

DataLayer
  -> EF Core persistence models and database access

WebApi
  -> HTTP API, request/response models, services, mappers, middleware
```

## Project Structure

The solution is split into three main projects:

```text
OrderManager.WebApi
OrderManager.Core
OrderManager.DataLayer
```

The solution also includes integration tests for validating the API behavior through real HTTP-style scenarios.

Recommended high-level structure:

```text
OrderManager.Core
├── Constants
├── Enums
├── Exceptions
├── Extensions
├── Helpers
└── Shared

OrderManager.DataLayer
├── BusinessObjects
│   ├── Non-Persistent
│   ├── Persistent
│   │   └── dbo
│   │       ├── BaseEntity.cs
│   │       ├── User.cs
│   │       └── Order.cs
│   └── Views
├── Configuration
├── Context
├── Interfaces
├── Migrations
├── Repositories
├── SQL Scripts
└── Utils

OrderManager.WebApi
├── Controllers
├── Interfaces
├── Mappers
├── Middleware
├── Models
│   ├── Auth
│   ├── Order
│   └── User
├── Services
├── Properties
├── appsettings.json
└── Program.cs
```

## Completed Recent Improvements

Recently completed improvements:

- Refactored the project structure to better match the company architecture
- Moved EF Core persistence classes into `DataLayer/BusinessObjects/Persistent/dbo`
- Replaced the previous `Core/Models` approach with DataLayer BusinessObjects and WebApi API models
- Moved API request/response models into `WebApi/Models`
- Moved API mappers into `WebApi/Mappers`
- Moved service interfaces into `WebApi/Interfaces`
- Moved repository interfaces into `DataLayer/Interfaces`
- Renamed the data access folder from `Data` to `Context`
- Kept `Core` focused on shared/common code such as constants and exceptions
- Added company-style `BusinessObjects` folders: `Persistent`, `Non-Persistent`, and `Views`
- Renamed the project to OrderManager
- Split user and admin routes in UsersController
- Split user and admin routes in OrdersController
- Added clearer `/me` user profile endpoints
- Added clearer `/admin` management endpoints
- Removed duplicate and unnecessary user lookup endpoints
- Removed public user creation from UsersController because registration is handled by AuthController
- Added password update support for the current user
- Added validation logic for old password and new password during profile updates
- Kept admin role assignment outside the public API
- Added integration tests for authentication, authorization, users, and orders
- Added JWT authentication
- Added password hashing
- Added AuthController
- Added Register and Login endpoints
- Protected endpoints with authorization
- Read the current user from JWT claims
- Removed UserId from regular order creation requests after authentication was implemented
- Added Swagger support for JWT bearer tokens

## Architecture

The project follows a company-style layered architecture.

### Request Flow

```text
HTTP Request
   -> Controller
   -> WebApi request model
   -> WebApi service
   -> WebApi mapper
   -> DataLayer BusinessObject
   -> DataLayer repository
   -> AppDbContext
   -> SQL Server Database
```

### Response Flow

```text
SQL Server Database
   -> AppDbContext
   -> DataLayer repository
   -> DataLayer BusinessObject
   -> WebApi service
   -> WebApi mapper
   -> WebApi response model
   -> Controller
   -> HTTP Response
```

### Model Types

The project intentionally separates API models from database persistence models.

```text
WebApi/Models
    Request and response models used by controllers and services.

DataLayer/BusinessObjects/Persistent/dbo
    EF Core persistence classes mapped to database tables.

Core
    Shared/common code such as constants, exceptions, enums, helpers, and extensions.
```

There is no separate `Core/Models` layer in the current structure. This keeps the project closer to the company convention where WebApi owns API models and DataLayer owns EF Core BusinessObjects.

## Projects

### OrderManager.WebApi

This is the startup project.

It contains everything related to the HTTP API:

- Controllers
- API request/response models
- API services
- API service interfaces
- API mappers
- Middleware
- Program.cs
- appsettings.json
- Swagger/OpenAPI configuration
- Dependency Injection setup
- Database provider configuration
- Authentication and authorization configuration

Responsibilities:

- Receive HTTP requests
- Validate API-level input
- Call services
- Return HTTP responses
- Register dependencies
- Configure middleware
- Configure Swagger
- Configure SQL Server through Entity Framework Core
- Configure JWT authentication
- Configure role-based authorization
- Map between API models and DataLayer BusinessObjects

### OrderManager.Core

This project contains shared/common code used by other projects.

It contains:

- Constants
- Enums
- Exceptions
- Extensions
- Helpers
- Shared utilities

Responsibilities:

- Define shared constants such as role names
- Define custom exceptions
- Hold reusable shared code
- Avoid direct dependency on WebApi or DataLayer

Core should not contain API DTOs, WebApi services, repository implementations, EF Core entities, or database configuration.

### OrderManager.DataLayer

This project contains persistence and data access code.

It contains:

- EF Core BusinessObjects
- AppDbContext
- Repository interfaces
- Repository implementations
- EF Core configuration
- EF Core migrations
- SQL scripts
- Data-layer utilities

Responsibilities:

- Communicate with SQL Server
- Use Entity Framework Core
- Define persistent database objects
- Configure database mappings, indexes, relationships, precision, and constraints
- Save, update, delete, and retrieve data
- Provide repository abstractions and implementations

## Dependency Direction

The projects depend on each other like this:

```text
WebApi -> Core
WebApi -> DataLayer
DataLayer -> Core
```

The Core project should not depend on WebApi or DataLayer.

The WebApi project can reference DataLayer because services and mappers work with DataLayer BusinessObjects in this company-style structure.

## BusinessObjects

BusinessObjects are EF Core persistence classes used by the DataLayer.

Current persistent BusinessObjects:

```text
DataLayer/BusinessObjects/Persistent/dbo/User.cs
DataLayer/BusinessObjects/Persistent/dbo/Order.cs
DataLayer/BusinessObjects/Persistent/dbo/BaseEntity.cs
```

### BusinessObjects Folder Meaning

```text
Persistent
    Classes mapped to real database tables.

Non-Persistent
    Classes used for query results, stored procedure results, or temporary database-related objects that are not directly mapped as normal tables.

Views
    Classes mapped to SQL views.
```

### User

A user represents a person who can authenticate and create orders.

Fields:

- Id
- Name
- Email
- PasswordHash
- Role
- Orders
- CreatedAt
- UpdatedAt

Rules:

- Name is required and limited to 50 characters.
- Email is required and limited to 100 characters.
- Email must be unique.
- Passwords are hashed before being stored.
- A newly registered user always receives the User role.
- The Admin role is not assigned through the public API.
- Admin access is assigned manually in the database.
- A user can have many orders.

### Order

An order represents a product ordered by a user.

Fields:

- Id
- Product
- Quantity
- Price
- Total
- UserId
- User
- CreatedAt
- UpdatedAt

Rules:

- Product is required and limited to 100 characters.
- Quantity must be greater than zero.
- Price must be greater than zero.
- Total is calculated by the backend.
- One order belongs to one user.
- Regular users can manage only their own orders.
- Admin users can manage all orders.

Relationship:

```text
One User can have many Orders.
One Order belongs to one User.
Orders.UserId is a foreign key to Users.Id.
```

When a user is deleted, the user's orders are deleted as well through cascade delete.

## WebApi Models

WebApi models are used to control what data enters and leaves the API.

Request models are used for API input.

Response models are used to control what data is returned from the API.

BusinessObjects are not returned directly from controllers. Responses should be built through WebApi mappers.

### User Models

Examples:

- UserRequestDto
- UserResponseDto
- UpdateUserResponseDto
- UpdateMyProfileRequestDto

`UpdateMyProfileRequestDto` is used when the currently authenticated user updates their own profile.

It supports regular profile updates:

- Name
- Email

It also supports optional password changes:

- OldPassword
- NewPassword

If the user wants to change the password, both old password and new password must be provided. The old password is verified before the new password is hashed and saved.

If old password and new password are not provided, only profile fields are updated.

### Auth Models

Examples:

- RegisterRequest
- LoginRequest
- AuthResponse

Auth models are used for registration, login, and returning JWT tokens after successful authentication.

User registration is handled by AuthController, not UsersController.

### Order Models

Examples:

- CreateOrderRequest
- AdminCreateOrderRequest
- UpdateOrderRequestDto
- OrderResponseDto

The regular order create request contains only the fields needed to create an order:

- Product
- Quantity
- Price

For regular users, UserId is read from JWT claims and is not trusted from the request body.

Admin order creation can include a UserId because admins are allowed to create orders for other users.

The order update request allows updating:

- Product
- Quantity
- Price

The following fields are not updated directly by the client:

- Id
- UserId
- Total
- CreatedAt
- UpdatedAt

Total is recalculated by the service whenever an order is created or updated.

## Repositories

Repositories are responsible for database access.

Repository interfaces are defined in:

```text
OrderManager.DataLayer/Interfaces
```

Repository implementations are defined in:

```text
OrderManager.DataLayer/Repositories
```

Repositories work with DataLayer BusinessObjects such as:

```text
User
Order
```

Example responsibilities:

- Get all users
- Get user by id
- Get user by email
- Add user
- Update user
- Delete user
- Search users
- Get all orders
- Get order by id
- Get orders by user id
- Add order
- Update order
- Delete order

Repositories use Entity Framework Core and AppDbContext.

## Services

Services contain business logic and application workflow.

Service interfaces are defined in:

```text
OrderManager.WebApi/Interfaces
```

Service implementations are defined in:

```text
OrderManager.WebApi/Services
```

Example responsibilities:

- Validate business rules
- Check if email already exists
- Check if a user exists before creating an order
- Check if an order exists before updating or deleting it
- Calculate order total
- Verify the old password before changing a user's password
- Hash the new password before saving it
- Call repositories
- Map BusinessObjects to response models
- Throw custom exceptions when something is invalid

Controllers should stay thin, and business logic should stay in services.

## Mappers

Mappers convert between WebApi models and DataLayer BusinessObjects.

Mappers are located in:

```text
OrderManager.WebApi/Mappers
```

Examples:

```text
User -> UserResponseDto
User -> UpdateUserResponseDto
Order -> OrderResponseDto
CreateOrderRequest -> Order
UserRequestDto -> User
```

This helps avoid exposing persistence objects directly through the API response shape.

For create operations, mappers can create a new BusinessObject from a request model.

For update operations, the existing BusinessObject is loaded from the database first, and then updated with values from the request model.

There are no DataLayer mappers in the current structure because the project no longer uses a separate Core model layer.

## Middleware

The project contains custom middleware.

### RequestLoggingMiddleware

Logs:

- HTTP method
- Request path
- Response status code

### Global Exception Handling

The application handles custom exceptions globally and returns proper HTTP responses.

Examples:

```text
BadRequestException -> 400 Bad Request
NotFoundException -> 404 Not Found
ConflictException -> 409 Conflict
Unhandled Exception -> 500 Internal Server Error
```

This keeps controllers clean and avoids repeating error handling logic in every endpoint.

## Swagger

Swagger is enabled for testing and documenting the API.

When the application is running, Swagger can be opened at:

```text
/swagger
```

Swagger allows testing endpoints directly from the browser.

Swagger is also configured to support JWT bearer authentication. After logging in and receiving a token, the token can be added through the Swagger Authorize button and used to test protected endpoints.

Swagger/OpenAPI packages belong in the WebApi project.

## Configuration

The project uses appsettings.json for configuration.

Example configuration values:

- SQL Server connection string
- Logging settings
- JWT issuer, audience, and secret key
- Application-specific values

Configuration is read in Program.cs and services where needed.

Example connection string section:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=OrderManagerDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

The application uses this connection string to connect to SQL Server through Entity Framework Core.

## Entity Framework Core

The project uses Entity Framework Core with SQL Server.

The application was originally using an InMemory database for learning purposes, but it has been migrated to a real SQL Server database.

EF Core is used for:

- DbContext configuration
- BusinessObject mapping
- Relationships
- Foreign keys
- Unique indexes
- Decimal precision
- Migrations
- Database updates

### AppDbContext Configuration

AppDbContext lives in:

```text
OrderManager.DataLayer/Context
```

EF Core configuration lives in:

```text
OrderManager.DataLayer/Configuration
```

Current configuration includes:

- Unique index on User.Email
- One-to-many relationship between User and Order
- Cascade delete from User to Orders
- Max length configuration for Name, Email, and Product
- Decimal precision for Price and Total
- datetime2 precision for CreatedAt and UpdatedAt

### Decimal Precision

Order price and total are configured with decimal precision:

```text
decimal(18,2)
```

This is used for storing monetary values in SQL Server.

### Audit Fields

The project includes audit fields:

```text
CreatedAt
UpdatedAt
```

CreatedAt is set when an entity is created.

UpdatedAt is set when an entity is modified.

This logic is handled centrally in AppDbContext through SaveChangesAsync.

### Migrations

EF Core migrations are used to create and update the database schema.

Common commands:

```powershell
Add-Migration InitialCreate -Project OrderManager.DataLayer -StartupProject OrderManager.WebApi
```

```powershell
Update-Database -Project OrderManager.DataLayer -StartupProject OrderManager.WebApi
```

The database can be inspected through SQL Server Management Studio.

Expected database tables:

```text
Users
Orders
__EFMigrationsHistory
```

The `__EFMigrationsHistory` table is created by Entity Framework Core and stores information about applied migrations.

## API Behavior

The API currently supports authentication, user management, and order management.

### Authentication

Authentication is implemented using JWT bearer tokens.

Supported operations include:

```text
POST /auth/register
POST /auth/login
```

Authentication behavior:

- Register a new user
- Login with email and password
- Return a JWT token after successful login
- Use JWT claims to identify the current user
- Use roles to protect admin-only endpoints
- Assign the User role to newly registered users
- Keep Admin role assignment outside the public API

Passwords are not stored as plain text. Password hashing is used before saving user credentials.

### Users

UsersController is split into regular user endpoints and admin endpoints.

Regular user endpoints:

```text
GET /users/me
PUT /users/me
```

Regular user behavior:

- A regular user can view their own profile.
- A regular user can update their own name and email.
- A regular user can optionally change their password.
- To change password, the user must provide both old password and new password.
- The old password is verified before the password is changed.
- If password fields are not provided, only name and email are updated.

Admin user endpoints:

```text
GET    /users/admin
GET    /users/admin/{userId}
GET    /users/admin/search?search=value&page=1&pageSize=10
PUT    /users/admin/{userId}
DELETE /users/admin/{userId}
```

Admin user behavior:

- Admins can get all users.
- Admins can get a specific user by id.
- Admins can search users with pagination.
- Admins can update users.
- Admins can delete users.
- Admin endpoints are protected with role-based authorization.

Removed user endpoints:

```text
POST /users
GET  /users/{userId}
PUT  /users/{userId}
GET  /users/email/{userEmail}
GET  /users/name/{userName}
GET  /users/names
```

Reasons:

- User registration belongs in AuthController.
- Regular users should use `/users/me`.
- Admin operations should use `/users/admin/...`.
- Search replaces separate email/name lookup endpoints.
- The controller is clearer with separate user and admin routes.

User email is normalized before saving.

Duplicate emails are rejected with a conflict response.

### Orders

OrdersController is split into regular user endpoints and admin endpoints.

Regular user endpoints:

```text
POST   /orders/my
GET    /orders/my
GET    /orders/my/{orderId}
PUT    /orders/my/{orderId}
DELETE /orders/my/{orderId}
```

Regular user behavior:

- Regular users can create orders for themselves.
- Regular users can get their own orders.
- Regular users can get one of their own orders by id.
- Regular users can update only their own orders.
- Regular users can delete only their own orders.
- UserId is read from JWT claims.

Admin order endpoints:

```text
POST   /orders/admin
GET    /orders/admin
GET    /orders/admin/{orderId}
GET    /orders/admin/user/{userId}
PUT    /orders/admin/{orderId}
DELETE /orders/admin/{orderId}
```

Admin order behavior:

- Admins can create orders for any user.
- Admins can get all orders.
- Admins can get any order by id.
- Admins can get orders for a specific user.
- Admins can update any order.
- Admins can delete any order.
- Admin endpoints are protected with role-based authorization.

Order total is calculated by the backend:

```text
Total = Quantity * Price
```

The client does not directly control the Total field.

## Integration Tests

The project includes integration tests for the main API behavior.

The tests cover authentication, authorization, user profile management, admin user management, and order management.

Example tested scenarios:

- Registering a user
- Logging in
- Accessing protected endpoints without a token
- Accessing admin endpoints with a regular user token
- Accessing admin endpoints with an admin token
- Getting the current user's profile
- Updating the current user's profile
- Updating the current user's password
- Rejecting password updates when the old password is incorrect
- Creating orders as the current user
- Getting the current user's orders
- Preventing regular users from accessing another user's orders
- Allowing admins to manage all orders

These tests help verify that routing, controllers, authentication, authorization, services, repositories, and database behavior work together correctly.

## Main Learning Goals

This project demonstrates:

- ASP.NET Core Web API basics
- Company-style layered architecture
- WebApi / Core / DataLayer project separation
- DataLayer BusinessObjects for EF Core persistence
- WebApi models for request/response shapes
- Controller -> Service -> Repository pattern
- Dependency Injection
- API model usage
- API validation
- Entity Framework Core
- SQL Server integration
- EF Core migrations
- DbContext configuration
- One-to-many relationships
- Foreign keys
- Unique indexes
- Decimal precision
- Audit fields
- Global exception handling
- Custom middleware
- Swagger/OpenAPI
- JWT authentication
- Password hashing
- Register and login endpoints
- Role-based authorization
- User and admin endpoint separation
- Current user endpoints with `/me`
- Admin management endpoints with `/admin`
- JWT claims usage
- Swagger JWT bearer token testing
- Password update with old password verification
- Integration testing
- Separation of concerns
- Async/await usage
- CancellationToken usage
- Basic production-style backend practices

## Planned Improvements

Planned future improvements:

- Soft delete for users and orders
- Refresh tokens