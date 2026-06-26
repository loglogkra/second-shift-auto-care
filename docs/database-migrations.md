# Service request database migrations

The Azure Functions API uses Entity Framework Core migrations and reads the SQL Server connection string from `SqlConnectionString`.

## Local development

1. Copy `src/SecondShiftAutoCare.Api/local.settings.sample.json` to `src/SecondShiftAutoCare.Api/local.settings.json`.
2. Replace the placeholder `SqlConnectionString` value with your local SQL Server connection string. Do not commit `local.settings.json`.
3. Apply the migrations from the repository root:

```bash
dotnet ef database update --project src/SecondShiftAutoCare.Api/SecondShiftAutoCare.Api.csproj --startup-project src/SecondShiftAutoCare.Api/SecondShiftAutoCare.Api.csproj --context ServiceRequestDbContext
```

## Azure SQL / production

1. Set the Function App application setting named `SqlConnectionString` to the Azure SQL connection string.
2. Apply migrations from a trusted deployment machine or CI/CD runner that has network access to the Azure SQL Server:

```bash
dotnet ef database update --project src/SecondShiftAutoCare.Api/SecondShiftAutoCare.Api.csproj --startup-project src/SecondShiftAutoCare.Api/SecondShiftAutoCare.Api.csproj --context ServiceRequestDbContext
```

Alternatively, generate an idempotent SQL script and run it through your normal Azure SQL deployment process:

```bash
dotnet ef migrations script --idempotent --project src/SecondShiftAutoCare.Api/SecondShiftAutoCare.Api.csproj --startup-project src/SecondShiftAutoCare.Api/SecondShiftAutoCare.Api.csproj --context ServiceRequestDbContext --output database/service-request-migrations.sql
```
