# LibraryApi
**Production ready REST API application for a book library built with .NET 10.**

Backend system that manages a book library.  
**Architecture**: Clean Architecture, CQRS.  
**Features**: JWT Bearer authentication, role-based authorization, integration tests. 

## Tech Stack
### Language & Runtime
- C# 13
- .NET 10
### Framework
- ASP.NET Core 10 (Minimal APIs)
### Database & ORM
- PostgreSQL 16
- Entity Framework Core 10
### Architecture 
- Clean Architecture
- CQRS
### Libraries
- MediatR
- FluentValidation
- Asp.Versioning
### API Docs
- OpenAPI
- Scalar
### Auth
- JWT Bearer authentication
- Role-based authorization
### Reliability
- Rate Limiting
- Problem Details
### Testing
- xUnit
- Moq
- Testcontainers
- WebApplicationFactory
### CI/CD
- GitHub Actions


## Features
- Book management (create, read, list)
- Book filtering
- JWT-based login authentication
- Role-based access control
- Rate limiting protection
- Data validation with error messages
- Auto-generated API documentation
- API versioning
- Automated testing and CI pipeline


## Architecture
The project is structured as a Clean Architecture with 4 layers:
- Domain - includes entities and value objects
- Application - includes services, CQRS handlers (Commands/Queries), and validators
- Infrastructure - includes repositories and database context
- Presentation - includes Minimal API endpoints and composition root

References are set up as follows:  
Domain (core) ← Application ← Infrastructure/Presentation  
Dependencies point inward — outer layers reference inner layers, never the other way around.


## Requirements
- .NET 10 SDK
- Docker (for PostgreSQL container)
- Git
- IDE (Rider, Visual Studio, or VS Code)


## Getting Started

1. Clone the repository:
```bash
   git clone https://github.com/bonq13/LibraryApi.git
   cd LibraryApi
```

2. Start a PostgreSQL container:
```bash
   docker run --name librarydb \
  -e POSTGRES_DB=librarydb \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:16
```

3. Apply database migrations:
```bash
   dotnet ef database update --project LibraryApi.Infrastructure --startup-project LibraryApi
```

4. Run the application:
```bash
   dotnet run --project LibraryApi
```

The API will be available at `https://localhost:7015` and Scalar documentation at `/scalar/v1`.

## Testing

The project includes unit tests (xUnit + Moq) and integration tests (Testcontainers + PostgreSQL).

Ensure Docker is running, then run:
```bash
dotnet test
```


## API Endpoints

Full API documentation is available via Scalar UI at `/scalar/v1` when running locally.

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST   | `/login` | User login, returns JWT token |
| GET    | `/v1/books` | List all books |
| GET    | `/v1/books/{id}` | Get book by ID |
| GET    | `/v1/books/available` | List available (non-borrowed) books |
| POST   | `/v1/books` | Add a new book (Admin only) |
| GET    | `/me` | Get current authenticated user info |

The API supports URL path versioning (v1, v2).