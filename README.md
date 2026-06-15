# LearningProject1

LearningProject1 is a simple ASP.NET Core Web API project created for learning backend development.

The project demonstrates a layered architecture using Controllers, Services, Repositories, DTOs, Entity Framework Core, Dependency Injection, Swagger, and global middleware.

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
   -> DbContext / Database
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

- Controllers
- Middleware
- Program.cs
- appsettings.json
- Swagger/OpenAPI configuration

Responsibilities:

- Receive HTTP requests
- Call services
- Return HTTP responses
- Register dependencies
- Configure middleware
- Configure Swagger

### LearningProject1.Core

This project contains the main application logic and shared contracts.

It contains:

- Models
- DTOs
- Mappers
- Services
- Interfaces
- Custom exceptions

Responsibilities:

- Define domain models
- Define request and response DTOs
- Implement business logic
- Define repository and service interfaces
- Map entities to DTOs

### LearningProject1.DataLayer

This project contains data access code.

It contains:

- AppDbContext
- Repository implementations

Responsibilities:

- Communicate with the database
- Use Entity Framework Core
- Implement repository interfaces from Core
- Save and retrieve entities

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

- Id
- Name
- Email
- Orders

### Order

An order represents a product ordered by a user.

Fields:

- Id
- Product
- UserId
- User

Relationship:

```text
One User can have many Orders.
One Order belongs to one User.
```

## DTOs

DTOs are used to control what data enters and leaves the API.

Request DTOs:

- CreateUserDto
- UpdateUserDto
- CreateOrderDto

Response DTOs:

- UserResponseDto
- OrderResponseDto

Entities are not returned directly from controllers.

## Repositories

Repositories are responsible for database access.

Example responsibilities:

- Get all users
- Get user by id
- Get user by email
- Add user
- Update user
- Delete user
- Get orders by user id
- Add order

Repositories use Entity Framework Core and AppDbContext.

Repository interfaces are defined in the Core project. Repository implementations are defined in the DataLayer project.

## Services

Services contain business logic.

Example responsibilities:

- Validate input
- Check if email already exists
- Check if a user exists before creating an order
- Call repositories
- Map models to response DTOs
- Throw custom exceptions when something is invalid

Controllers should stay thin, and business logic should stay in services.

## Mappers

Mappers convert between models and DTOs.

Examples:

```text
User -> UserResponseDto
Order -> OrderResponseDto
CreateOrderDto -> Order
CreateUserDto -> User
```

This helps avoid exposing database entities directly through the API.

## Middleware

The project contains custom middleware.

### RequestLoggingMiddleware

Logs:

- HTTP method
- Request path
- Response status code

### GlobalExceptionHandlingMiddleware

Handles exceptions globally and returns proper HTTP responses.

Examples:

```text
BadRequestException -> 400 Bad Request
Unhandled Exception -> 500 Internal Server Error
```

## Swagger

Swagger is enabled for testing and documenting the API.

When the application is running, Swagger can be opened at:

```text
/swagger
```

Swagger allows you to test endpoints directly from the browser.

Swagger/OpenAPI packages belong in the WebApi project.

## Configuration

The project uses `appsettings.json` for configuration.

Example configuration values:

- Database name
- Logging settings
- Application-specific values

Configuration is read in `Program.cs`.

## Entity Framework Core

The project uses Entity Framework Core with an InMemory database.

This is useful for learning because no external database setup is required.

EF Core packages belong in the DataLayer project.

## Main Learning Goals

This project demonstrates:

- ASP.NET Core Web API basics
- Controller -> Service -> Repository pattern
- Dependency Injection
- DTO usage
- Entity Framework Core
- InMemory database
- One-to-many relationships
- Global exception handling
- Custom middleware
- Swagger/OpenAPI
- Clean separation of concerns
- Async/await usage
