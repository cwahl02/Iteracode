using FluentValidation;
using Iteracode.Api.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddCorsPolicy()
    .AddDatabaseExtensions(builder.Configuration)
    .AddIdentityServices()
    .AddJwtConfiguration(builder.Configuration)
    .AddEndpoints()
    .AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();