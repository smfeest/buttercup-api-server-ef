using Buttercup.Api;
using Buttercup.Api.DbModel;
using Buttercup.Api.Security;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CA1852

var builder = WebApplication.CreateBuilder(args);

var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddPooledDbContextFactory<AppDbContext>(
    options => options.UseAppDbOptions(builder.Configuration.GetConnectionString("AppDb")));

builder.Services
    .AddGraphQLServer()
    .AddProjections()
    .AddQueryType<Query>()
    .AddTypeExtension<UserExtension>()
    .AllowIntrospection(isDevelopment)
    .ModifyRequestOptions(options => options.IncludeExceptionDetails = isDevelopment)
    .RegisterDbContext<AppDbContext>(DbContextKind.Pooled);

builder.Services.AddDataProtection();

builder.Services
    .AddCommonServices()
    .AddSecurityServices();

var app = builder.Build();

app.MapGraphQL().WithOptions(new()
{
    EnableSchemaRequests = isDevelopment,
    Tool = { Enable = isDevelopment }
});

app.Run();
