using Iteracode.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi()
    .AddCorsPolicy()
    .AddDatabaseExtensions(builder.Configuration)
    .AddIdentityServices()
    .AddJwtConfiguration(builder.Configuration)
    .AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();