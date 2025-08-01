# LogService.Razor - Razor Pages Frontend

This project provides a Razor Pages-based frontend for the LogService application, designed to be more resource-efficient and better suited for unstable connections like mobile phones compared to the existing Blazor Server implementation.

## Key Features

### Authentication
- **Cookie-based Authentication**: Instead of storing JWT tokens in browser local storage (like the Blazor version), this implementation uses secure HTTP-only cookies
- **Secure Token Storage**: Tokens are stored server-side in cookies, reducing exposure to XSS attacks
- **Automatic Session Management**: Built-in cookie expiration and sliding expiration support

### Real-time Updates
- **SignalR Integration**: Maintains the SignalR connection for real-time log message updates
- **JavaScript Client**: Uses the SignalR JavaScript client to receive real-time updates and update the UI dynamically
- **User Groups**: Users join SignalR groups to receive only their relevant log messages

### Pages
- **Index**: Dashboard with user settings and service token generation
- **LogMessages**: Paginated list of log messages with filtering capabilities
- **LogMessage**: Detailed view of individual log messages
- **Account/Login**: Cookie-based login page
- **Account/Register**: User registration page
- **Account/Logout**: Logout functionality

### Benefits over Blazor Server
1. **Lower Resource Usage**: Traditional request/response model uses less server memory
2. **Better Mobile Performance**: Works better with intermittent connections
3. **Reduced SignalR Dependency**: Only uses SignalR for real-time updates, not for the entire UI
4. **Faster Initial Load**: Static HTML is faster to load than Blazor components
5. **Better SEO**: Server-rendered HTML is more SEO-friendly

## Implementation Details

### Cookie-based Authentication Services
- `CookieUserDataStore`: Implements `IUserDataStore` using HTTP context and claims
- `CookieTokenProvider`: Implements `ITokenProvider` for cookie-based token retrieval
- Authentication tokens are stored as claims in the authentication cookie

### SignalR Hub
- `LogHub`: Handles real-time log message broadcasting
- Users join groups based on their username
- Sends log message updates to connected clients

### Responsive Design
- Bootstrap 5 for responsive UI
- Mobile-optimized tables and forms
- Collapsible filter sections

## Configuration

The application uses the same configuration as the Blazor version:
- `DataServiceUrl`: URL to the LogService backend API
- Standard ASP.NET Core configuration for logging, authentication, etc.

## Dependencies

This project reuses the existing client libraries:
- `LogService.Client.Data`: Data access and service layer
- `LogService.Client.Localisation`: Localization resources
- Shared contracts and models from the existing solution

## Usage

1. Start the LogService backend API
2. Configure the `DataServiceUrl` in appsettings.json
3. Run the Razor Pages application
4. Navigate to the application and register/login
5. Use the same functionality as the Blazor version but with improved performance on mobile devices

## Future Enhancements

- Progressive Web App (PWA) support for offline capabilities
- Service Worker for background sync
- Enhanced mobile UI optimizations
- Improved filter persistence across sessions