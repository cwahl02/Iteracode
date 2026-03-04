using FluentValidation;
using Iteracode.Api.Extensions;
using Iteracode.Api.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services
    .AddOpenApi()
    .AddCorsPolicy()
    .AddDatabaseExtensions(builder.Configuration)
    .AddIdentityServices()
    .AddJwtConfiguration(builder.Configuration)
    .AddEndpoints()
    .AddValidatorsFromAssembly(typeof(Program).Assembly)
    .AddInjectionMarkers();

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();