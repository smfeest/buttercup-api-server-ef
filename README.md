# Buttercup API server

## Development environment

### Required software

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [PostgreSQL 14.5 or newer](https://www.postgresql.org/download/)

### Initial set up

1.  Restore .NET tools:

        dotnet tool restore

2.  Create the database user:

        sudo -u postgres psql --command="CREATE USER buttercup_dev PASSWORD 'buttercup_dev' CREATEDB"

3.  Create the application database:

        dotnet ef database update -s Buttercup.Api

### Useful commands

- Running the application, watching for changes:

      dotnet watch run --project Buttercup.Api

- Running the application, without watching for changes:

      dotnet run --project Buttercup.Api

- Running all tests, watching for changes:

      dotnet watch test --project buttercup-api-server.sln

- Running all tests, without watching for changes

      dotnet test

- Adding a new database migration:

      dotnet ef migrations add <NAME> \
        -s Buttercup.Api \
        -p Buttercup.Api.DbModel.Migrations

- Running all pending database migrations:

      dotnet ef database update -s Buttercup.Api

- Dropping the database:

      dotnet ef database drop -s Buttercup.Api
