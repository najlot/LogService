# LogService GitHub Copilot Instructions

## Project Overview

LogService is a centralized logging service built with .NET 9.0 that processes log messages from the `HttpDestination` of `Najlot.Log`. It provides a central point to collect, manage, and view logs from multiple applications with user-based segregation and real-time monitoring capabilities.

## Architecture

This is a multi-client architecture consisting of:

- **LogService.Service**: ASP.NET Core Web API backend with SignalR support
- **LogService.Blazor**: Blazor Server web application for log management UI
- **LogService.Wpf**: WPF desktop client for log viewing
- **LogService.Contracts**: Shared data contracts and models
- **LogService.Client.Data**: Shared data access layer
- **LogService.Client.MVVM**: MVVM infrastructure for client applications
- **LogService.ClientBase**: Base client functionality

### Data Storage Options

The service supports multiple storage backends:
- **MongoDB**: Primary NoSQL option
- **MySQL**: Relational database option with Entity Framework
- **File System**: Fallback file-based storage

## Technology Stack

### Backend Technologies
- **.NET 9.0**: Target framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core**: ORM for MySQL
- **MongoDB.Driver**: MongoDB integration
- **SignalR**: Real-time communication
- **JWT Authentication**: Token-based authentication
- **Dapper**: Lightweight ORM for performance-critical queries

### Frontend Technologies
- **Blazor Server**: Web UI framework
- **WPF**: Desktop application framework
- **MVVM Pattern**: Client architecture pattern

### Third-Party Libraries
- **Najlot.Log**: Core logging framework
- **Najlot.Map**: Object mapping
- **Cosei.Service**: Service infrastructure
- **System.Text.Json**: JSON serialization

## Development Guidelines

### Code Organization

```
src/
├── LogService.Service/           # Web API backend
│   ├── Controllers/             # API controllers
│   ├── Services/               # Business logic
│   ├── Repository/             # Data access
│   ├── Configuration/          # Configuration models
│   └── Mappings/              # Object mappings
├── LogService.Blazor/          # Web UI
│   ├── Pages/                 # Razor pages
│   ├── Services/              # Client services
│   └── Shared/                # Shared components
├── LogService.Wpf/             # Desktop client
├── LogService.Contracts/       # Shared contracts
├── LogService.Client.Data/     # Shared data layer
└── Tests/                      # Unit and integration tests
```

### Coding Standards

#### C# Conventions
- Use **file-scoped namespaces** for .NET 9.0
- Follow **PascalCase** for public members, **camelCase** for private/local
- Use **nullable reference types** consistently
- Prefer **record types** for immutable data contracts
- Use **primary constructors** where appropriate

#### API Development
- **RESTful design**: Use appropriate HTTP verbs and status codes
- **Async/await**: All I/O operations must be asynchronous
- **JWT Authentication**: Secure endpoints with `[Authorize]` attribute
- **Model validation**: Use data annotations and model validation
- **Error handling**: Return consistent error responses

#### Database Patterns
- **Repository pattern**: Implement `IUserRepository`, `ILogMessageRepository`
- **Dependency injection**: Register repositories in `Startup.cs`
- **Connection management**: Use scoped lifetime for database contexts
- **Query optimization**: Use Dapper for read-heavy operations

### Authentication & Security

#### JWT Implementation
```csharp
// Token validation setup
var validationParameters = TokenService.GetValidationParameters(secret);
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = validationParameters;
    });
```

#### User Segregation
- Users can only view logs from tokens they created
- Source-based log grouping for application segregation
- Token-based access control for log retrieval

### SignalR Integration

#### Real-time Features
- **Live log streaming**: Push new log messages to connected clients
- **Connection management**: Handle client connections and groups
- **Hub configuration**: Use `CoseiHub` for real-time communication

```csharp
// SignalR setup
services.AddSignalR();
endpoints.MapHub<CoseiHub>("/cosei");
```

### Data Models

#### Core Entities
```csharp
public class LogMessage
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public LogLevel LogLevel { get; set; }
    public string Category { get; set; }
    public string Source { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }
    public List<LogArgument> Arguments { get; set; }
}
```

#### Configuration Models
- Implement configuration classes for each storage provider
- Use `IConfiguration.ReadConfiguration<T>()` for type-safe config binding
- Validate required configuration on startup

### Multi-Storage Backend Pattern

```csharp
// Storage provider selection in Startup.cs
if (mongoDbConfig != null)
{
    services.AddScoped<IUserRepository, MongoDbUserRepository>();
    services.AddScoped<ILogMessageRepository, MongoDbLogMessageRepository>();
}
else if (mysqlConfig != null)
{
    services.AddScoped<IUserRepository, MySqlUserRepository>();
    services.AddScoped<ILogMessageRepository, MySqlLogMessageRepository>();
}
else
{
    services.AddScoped<IUserRepository, FileUserRepository>();
    services.AddScoped<ILogMessageRepository, FileLogMessageRepository>();
}
```

### Client Development

#### Blazor Server Guidelines
- **Component-based architecture**: Create reusable Razor components
- **State management**: Use scoped services for page-level state
- **Real-time updates**: Integrate with SignalR for live data
- **Authentication**: Implement proper login/logout flows

#### WPF MVVM Pattern
- **ViewModels**: Implement `INotifyPropertyChanged` for data binding
- **Commands**: Use `ICommand` for user interactions
- **Services**: Inject HTTP clients and data services
- **Async operations**: Use async/await with proper error handling

### Testing Guidelines

#### Unit Testing
- **Repository tests**: Mock database dependencies
- **Service tests**: Test business logic in isolation
- **Controller tests**: Test API endpoints with mock services
- **Integration tests**: Test complete request/response cycles

#### Test Structure
```csharp
[Fact]
public async Task GetLogMessages_ReturnsFilteredResults()
{
    // Arrange
    var mockRepo = new Mock<ILogMessageRepository>();
    var service = new LogMessageService(mockRepo.Object);
    
    // Act
    var result = await service.GetLogMessagesAsync(filter);
    
    // Assert
    Assert.NotNull(result);
}
```

### Performance Considerations

#### Database Optimization
- **Indexing**: Create indexes on frequently queried fields (DateTime, Source, LogLevel)
- **Pagination**: Implement proper pagination for large log datasets
- **Caching**: Use memory caching for frequently accessed data
- **Cleanup**: Implement log retention policies with `LogMessageCleanUpService`

#### API Performance
- **Async operations**: All database operations must be async
- **Response compression**: Enable compression for large datasets
- **Query optimization**: Use efficient queries with proper filtering

### Configuration Management

#### appsettings.json Structure
```json
{
  "ServiceConfiguration": {
    "Secret": "your-jwt-secret-key"
  },
  "MongoDbConfiguration": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "LogService"
  },
  "MySqlConfiguration": {
    "ConnectionString": "Server=localhost;Database=LogService;..."
  }
}
```

#### Environment-Specific Config
- Use `appsettings.development.json` for development settings
- Implement configuration validation on startup
- Use environment variables for sensitive configuration

### Deployment Considerations

#### Docker Support
- Container-ready architecture with configurable storage backends
- Health checks for monitoring service status
- Environment variable configuration

#### CORS Configuration
```csharp
app.UseCors(c => {
    c.AllowAnyOrigin();
    c.AllowAnyMethod();
    c.AllowAnyHeader();
});
```

### Common Patterns

#### Error Handling
```csharp
try
{
    var result = await repository.GetAsync(id);
    return Ok(result);
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to retrieve data");
    return StatusCode(500, "Internal server error");
}
```

#### Dependency Injection
- Register all dependencies in `Startup.ConfigureServices`
- Use appropriate lifetimes: Singleton for config, Scoped for repositories
- Implement factory patterns for multi-provider scenarios

#### Logging Integration
- Use structured logging with `ILogger<T>`
- Include correlation IDs for request tracking
- Log at appropriate levels (Debug, Info, Warning, Error)

### Best Practices

#### Security
- **Input validation**: Validate all user inputs
- **SQL injection prevention**: Use parameterized queries
- **Authentication**: Verify JWT tokens on protected endpoints
- **Authorization**: Implement proper access controls

#### Maintainability
- **Separation of concerns**: Keep controllers thin, business logic in services
- **Single responsibility**: Each class should have one reason to change
- **Dependency inversion**: Depend on abstractions, not concrete implementations
- **Documentation**: Document complex business logic and API endpoints

#### Performance
- **Lazy loading**: Load data only when needed
- **Connection pooling**: Reuse database connections efficiently
- **Memory management**: Dispose resources properly
- **Concurrent operations**: Use async/await for I/O bound operations

This architecture provides a scalable, maintainable logging service that can handle multiple clients and storage backends while maintaining security and performance standards.