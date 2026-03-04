using Iteracode.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi()
    .AddCorsPolicy()
    .AddDatabaseExtensions(builder.Configuration)
    .AddIdentityServices();

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();