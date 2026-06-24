# Database migrations

Run `001_create_service_requests.sql` against Azure SQL before deploying the Functions API.

The API expects a connection string named `SqlConnection` (standard .NET connection string configuration) or an app setting named `SqlConnectionString`.
