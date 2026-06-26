# Database migrations

The service request database schema is managed with Entity Framework Core migrations in `src/SecondShiftAutoCare.Api/Migrations`.

The API expects a setting named `SqlConnectionString` in local configuration or Azure Function App application settings.

See `docs/database-migrations.md` for local and Azure SQL migration commands.
