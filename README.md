# Log.Service

This service is developed to process log messages from the `HttpDestination` of `Najlot.Log`,
providing a central point to collect and manage logs.

## Configuration in Najlot.Log

To send log messages to this service, you need to configure `Najlot.Log` by adding the following call:

```csharp
.AddHttpDestination("http://HOSTNAME:PORT/api/LogMessage", "<token>")
```

Replace `HOSTNAME` and `PORT` with the appropriate values for your setup, and include your authentication token.

# Segregation by User
It is possible to register on an instance of this service, and when creating the token, you can specify a "Source".

The idea behind the "Source" is to represent the application sending the log messages.
The logged-in user can only view log messages from tokens that were created by this user.
This allows services to be grouped by user.

This design also supports the possibility of separating service groups by creating a new user,
making it easier to manage logs in an environment with many different.

# Next steps
- Cleaning of Messages after a defined amount of time
- Filtering
- UI-Design