# Database migrations

The service request database schema is managed with Entity Framework Core migrations in `src/SecondShiftAutoCare.Api/Migrations`.

The API expects a setting named `SqlConnectionString` in local configuration or Azure Function App application settings.

See `docs/database-migrations.md` for local and Azure SQL migration commands.

- `002_expand_service_requests.sql` expands intake/admin fields for guided service requests, urgency, drivable/location, alternate contacts, photo follow-up, and archived request support.

- `003_add_quote_builder_fields.sql` adds quote-builder pricing, templates, quote options, expiration, and approval status fields.
