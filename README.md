# LearningProject

LearningProject1 is an ASP.NET Core Web API project created for learning backend development with a focus on production-style backend practices.

The project demonstrates a layered architecture using Controllers, Services, Repositories, DTOs, Entity Framework Core, SQL Server, Dependency Injection, Swagger, custom middleware, global exception handling, and EF Core migrations.

## Project Structure

The solution is split into three projects:

```text
LearningProject1.WebApi
LearningProject1.Core
LearningProject1.DataLayer
```

## Architecture

The project follows this request flow:

```text
HTTP Request
   -> Controller
   -> Request DTO
   -> Service
   -> Mapper
   -> Entity / Model
   -> Repository
   -> DbContext / SQL Server Database
```

The response flows back like this:

```text
Database Entity
   -> Repository
   -> Service
   -> Mapper
   -> Response DTO
   -> Controller
   -> HTTP Response
```

## Projects

### LearningProject1.WebApi

This is the startup project.

It contains everything related to the HTTP API:

* Controllers
* Middleware
* Program.cs
* appsettings.json
* Swagger/OpenAPI configuration
* Dependency Injection setup
* Database provider configuration

Responsibilities:

* Receive HTTP requests
* Call services
* Return HTTP responses
* Register dependencies
* Configure middleware
* Configure Swagger
* Configure SQL Server through Entity Framework Core

### LearningProject1.Core

This project contains the main application logic and shared contracts.

It contains:

* Models
* DTOs
* Mappers
* Services
* Interfaces
* Custom exceptions

Responsibilities:

* Define domain models
* Define request and response DTOs
* Implement business logic
* Define repository and service interfaces
* Map entities to DTOs
* Validate business rules
* Throw custom exceptions when something is invalid

### LearningProject1.DataLayer

This project contains data access code.

It contains:

* AppDbContext
* Repository implementations
* EF Core database configuration
* EF Core migrations

Responsibilities:

* Communicate with the SQL Server database
* Use Entity Framework Core
* Implement repository interfaces from Core
* Save, update, delete, and retrieve entities
* Configure database relationships, indexes, precision, and constraints

## Dependency Direction

The projects depend on each other like this:

```text
WebApi -> Core
WebApi -> DataLayer
DataLayer -> Core
```

The Core project does not depend on WebApi or DataLayer.

This keeps the business logic separated from the API and database implementation.

## Domain Models

### User

A user represents a person who can create orders.

Fields:

* Id
* Name
* Email
* Orders
* CreatedAt
* UpdatedAt

Rules:

* Name is required and limited to 50 characters.
* Email is required and limited to 100 characters.
* Email must be unique.
* A user can have many orders.

### Order

An order represents a product ordered by a user.

Fields:

* Id
* Product
* Quantity
* Price
* Total
* UserId
* User
* CreatedAt
* UpdatedAt

Rules:

* Product is required and limited to 100 characters.
* Quantity must be greater than zero.
* Price must be greater than zero.
* Total is calculated by the backend.
* One order belongs to one user.

Relationship:

```text
One User can have many Orders.
One Order belongs to one User.
Orders.UserId is a foreign key to Users.Id.
```

When a user is deleted, the user's orders are deleted as well through cascade delete.

## DTOs

DTOs are used to control what data enters and leaves the API.

Request DTOs are used for input validation and API requests.

Response DTOs are used to control what data is returned from the API.

Entities are not returned directly from controllers.

### User DTOs

Examples:

* UserRequestDto
* UserResponseDto
* UpdateUserResponseDto

### Order DTOs

Examples:

* OrderRequestDto
* UpdateOrderRequestDto
* OrderResponseDto

The order create request contains the fields required to create an order.

The order update request allows updating:

* Product
* Quantity
* Price

The following fields are not updated directly by the client:

* Id
* UserId
* Total
* CreatedAt
* UpdatedAt

Total is recalculated by the service whenever an order is created or updated.

## Repositories

Repositories are responsible for database access.

Example responsibilities:

* Get all users
* Get user by id
* Get user by email
* Add user
* Update user
* Delete user
* Get all orders
* Get order by id
* Get orders by user id
* Add order
* Update order
* Delete order

Repositories use Entity Framework Core and AppDbContext.

Repository interfaces are defined in the Core project.

Repository implementations are defined in the DataLayer project.

## Services

Services contain business logic.

Example responsibilities:

* Validate business rules
* Check if email already exists
* Check if a user exists before creating an order
* Check if an order exists before updating or deleting it
* Calculate order total
* Call repositories
* Map models to response DTOs
* Throw custom exceptions when something is invalid

Controllers should stay thin, and business logic should stay in services.

## Mappers

Mappers convert between models and DTOs.

Examples:

```text
User -> UserResponseDto
Order -> OrderResponseDto
OrderRequestDto -> Order
UpdateOrderRequestDto -> existing Order
UserRequestDto -> User
```

This helps avoid exposing database entities directly through the API.

For create operations, mappers can create a new entity from a request DTO.

For update operations, the existing entity is loaded from the database first, and then updated with values from the request DTO.

## Middleware

The project contains custom middleware.

### RequestLoggingMiddleware

Logs:

* HTTP method
* Request path
* Response status code

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

Swagger/OpenAPI packages belong in the WebApi project.

## Configuration

The project uses appsettings.json for configuration.

Example configuration values:

* SQL Server connection string
* Logging settings
* Application-specific values

Configuration is read in Program.cs.

Example connection string section:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=LearningProjectDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

The application uses this connection string to connect to SQL Server through Entity Framework Core.

## Entity Framework Core

The project uses Entity Framework Core with SQL Server.

The application was originally using an InMemory database for learning purposes, but it has been migrated to a real SQL Server database.

EF Core is used for:

* DbContext configuration
* Entity mapping
* Relationships
* Foreign keys
* Unique indexes
* Decimal precision
* Migrations
* Database updates

### AppDbContext Configuration

AppDbContext configures the database model using Fluent API.

Current configuration includes:

* Unique index on User.Email
* One-to-many relationship between User and Order
* Cascade delete from User to Orders
* Max length configuration for Name, Email, and Product
* Decimal precision for Price and Total
* datetime2 precision for CreatedAt and UpdatedAt

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
Add-Migration InitialCreate -Project LearningProject1.DataLayer -StartupProject LearningProject1.WebApi
```

```powershell
Update-Database -Project LearningProject1.DataLayer -StartupProject LearningProject1.WebApi
```

The database can be inspected through SQL Server Management Studio.

Expected database tables:

```text
Users
Orders
__EFMigrationsHistory
```

The __EFMigrationsHistory table is created by Entity Framework Core and stores information about applied migrations.

## API Behavior

The API currently supports user and order management.

### Users

Supported operations include:

* Get all users
* Get user by id
* Get user by email
* Get users by name
* Search users
* Create user
* Update user
* Delete user

User email is normalized before saving.

Duplicate emails are rejected with a conflict response.

### Orders

Supported operations include:

* Get all orders
* Get order by id
* Get orders by user id
* Create order
* Update order
* Delete order

Order total is calculated by the backend:

```text
Total = Quantity * Price
```

The client does not directly control the Total field.

## Main Learning Goals

This project demonstrates:

* ASP.NET Core Web API basics
* Controller -> Service -> Repository pattern
* Dependency Injection
* DTO usage
* Entity Framework Core
* SQL Server integration
* EF Core migrations
* DbContext configuration with Fluent API
* One-to-many relationships
* Foreign keys
* Unique indexes
* Decimal precision
* Audit fields
* Global exception handling
* Custom middleware
* Swagger/OpenAPI
* Clean separation of concerns
* Async/await usage
* CancellationToken usage
* Basic production-style backend practices

## Next Learning Goals

Planned next steps:

* Add JWT authentication
* Add password hashing
* Add AuthController
* Add Register and Login endpoints
* Protect order endpoints with authorization
* Read the current user from JWT claims
* Remove UserId from order creation requests after authentication is implemented
* Add Swagger support for JWT bearer tokens
* Add integration tests
