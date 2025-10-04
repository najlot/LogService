# LogService

LogService is a unified centralized logging service built with .NET 9.0 that processes log messages from the `HttpDestination` of `Najlot.Log`. It combines a Web API backend with a Blazor Server web interface in a single deployable service, providing:

- **Centralized log collection** via REST API
- **Web-based log management** with Blazor Server UI
- **Real-time log streaming** via SignalR
- **User-based log segregation** for multi-tenant scenarios
- **Multiple storage backends** (MongoDB, MySQL, File System)

## Architecture

The service is designed as a **single deployable application** that includes:
- ASP.NET Core Web API for log ingestion and management
- Blazor Server web interface for viewing and managing logs
- SignalR hub for real-time log updates
- JWT-based authentication and authorization

## Configuration in Najlot.Log

To send log messages to this service, you need to configure `Najlot.Log` by adding the following call:

```csharp
.AddHttpDestination("http://HOSTNAME:PORT/api/LogMessage", "<token>")
```

Replace `HOSTNAME` and `PORT` with the appropriate values for your setup (default port is 5000), and include your authentication token.

## Segregation by User

The service supports multi-tenant operation:
- Register users on the service instance
- Each user can create tokens with a "Source" identifier
- The "Source" represents the application sending log messages
- Users can only view log messages from tokens they created
- Separate service groups by creating different users

This design makes it easier to manage logs in environments with many different service groups.

## Deployment

Simply deploy the single **LogService** application:
1. Configure your preferred storage backend in `appsettings.json`
2. Set the JWT secret in configuration
3. Run the service on port 5000 (or configure as needed)
4. Access the web UI at `http://HOSTNAME:PORT/`
5. API endpoints are available at `http://HOSTNAME:PORT/api/`

## Storage Backends

The service supports multiple storage options:
- **MongoDB**: For NoSQL document storage
- **MySQL**: For relational database storage with Entity Framework
- **File System**: For simple file-based storage (fallback)

Configure your preferred backend in `appsettings.json`.

## Next Steps

- Filtering on new messages from SignalR
- Filter improvements
- Enhanced search capabilities