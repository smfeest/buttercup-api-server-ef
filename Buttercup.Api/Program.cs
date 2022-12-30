using Buttercup.Api;
using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CA1852

var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddDbContext<AppDbContext>(
    options => options
        .UseNpgsql(
            builder.Configuration.GetConnectionString("AppDb"),
            npgsqlOptions => npgsqlOptions
                .MigrationsAssembly("Buttercup.Api.DbModel.Migrations")
                .MigrationsHistoryTable("__migration_history"))
        .UseSnakeCaseNamingConvention());

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AllowIntrospection(isDevelopment)
    .ModifyRequestOptions(options => options.IncludeExceptionDetails = isDevelopment)
    .RegisterDbContext<AppDbContext>();

var app = builder.Build();

app.MapGraphQL().WithOptions(new()
{
    EnableSchemaRequests = isDevelopment,
    Tool = { Enable = isDevelopment }
});

app.Run();
