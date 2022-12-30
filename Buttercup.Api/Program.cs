using Buttercup.Api;

#pragma warning disable CA1852

var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AllowIntrospection(isDevelopment);

var app = builder.Build();

app.MapGraphQL().WithOptions(new()
{
    EnableSchemaRequests = isDevelopment,
    Tool = { Enable = isDevelopment }
});

app.Run();
