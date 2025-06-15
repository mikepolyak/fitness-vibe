# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

FitnessVibe is a gamified fitness tracking application built with a Clean Architecture approach using:
- **Frontend**: Angular 17 + TypeScript with NgRx for state management
- **Backend**: .NET 8 + C# with CQRS pattern using MediatR
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT with refresh tokens

## Architecture

The backend follows Clean Architecture with clear separation of concerns:

```
src/backend/
├── FitnessVibe.Domain/        # Core business logic & entities
├── FitnessVibe.Application/   # Use cases, commands, queries, handlers
├── FitnessVibe.Infrastructure/# Data access, external services
└── FitnessVibe.API/          # Controllers, middleware, startup
```

The frontend uses Angular feature modules with lazy loading:
```
src/frontend/app/
├── core/        # Singleton services, guards, interceptors
├── shared/      # Reusable components, pipes, models
├── features/    # Feature modules (auth, dashboard, activities, etc.)
└── store/       # NgRx state management (actions, reducers, effects)
```

## Common Development Commands

### Setup
```bash
npm run setup              # Setup both frontend and backend
npm run setup:frontend     # Install frontend dependencies
npm run setup:backend      # Restore backend packages
```

### Development
```bash
npm start                  # Start both frontend and backend concurrently
npm run start:frontend     # Start Angular dev server (port 4200)
npm run start:backend      # Start .NET API (port 5001)
```

### Building
```bash
npm run build              # Build both frontend and backend
npm run build:frontend     # Build Angular app for production
npm run build:backend      # Build .NET solution in Release mode
```

### Testing
```bash
npm test                   # Run all tests (frontend + backend)
npm run test:frontend      # Run Angular tests (Karma + Jasmine)
npm run test:backend       # Run .NET tests (xUnit)
npm run test:coverage      # Generate coverage reports for both
```

### Linting & Formatting
```bash
npm run lint               # Lint frontend code (ESLint)
npm run lint:fix           # Auto-fix linting issues
npm run format             # Format frontend code (Prettier)
```

### Database Migrations
```bash
npm run migration:add <name>     # Add new EF Core migration
npm run migration:update         # Apply migrations to database
npm run migration:remove         # Remove last migration
```

### Docker
```bash
npm run docker:build       # Build Docker containers
npm run docker:up          # Start application with Docker Compose
npm run docker:down        # Stop Docker containers
```

## Key Patterns & Conventions

### Backend (C#)
- **CQRS**: Commands and Queries are separate with dedicated handlers
- **MediatR**: All application logic flows through MediatR handlers
- **Repository Pattern**: Data access abstracted through repositories
- **Domain Events**: Domain entities can raise events for cross-cutting concerns
- **Validation**: FluentValidation for input validation
- **Mapping**: AutoMapper for object mapping between layers

### Frontend (Angular)
- **Feature Modules**: Each major feature has its own module with routing
- **NgRx**: Centralized state management with actions, reducers, effects
- **Smart/Dumb Components**: Container components manage state, presentation components are pure
- **Services**: Business logic in services, components focus on presentation
- **Guards**: Route protection and user authorization
- **Interceptors**: HTTP request/response handling (auth, errors, loading)

### Entity Framework Conventions
- All entities inherit from `BaseEntity` with common properties (Id, CreatedAt, UpdatedAt)
- Entities implementing `IUserOwnedEntity` require user ownership validation
- Configurations are in separate files in `Infrastructure/Data/Configurations/`
- Repository implementations handle complex queries and business-specific data operations

### Authentication & Authorization
- JWT tokens with refresh token rotation
- Role-based authorization using attributes
- User context available through dependency injection
- Frontend interceptors handle token refresh automatically

## Development Notes

- The application uses Clean Architecture principles - always respect layer boundaries
- Frontend components should be tested with Jasmine/Karma
- Backend handlers should have corresponding unit tests with xUnit
- Use the existing validation patterns when adding new commands/queries
- Follow the established naming conventions for consistency
- Database changes require migrations - never modify the database directly
- Use the existing error handling patterns (custom exceptions, global error handling)