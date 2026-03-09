using FluentValidation;
using Iteracode.Api.Extensions;
using Iteracode.Api.Options;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services
    .AddOpenApi()
    .AddCorsPolicy()
    .AddDatabaseExtensions()
    .AddIdentityServices()
    .AddJwtConfiguration()
    .AddEndpoints()
    .AddValidatorsFromAssembly(typeof(Program).Assembly)
    .AddInjectionMarkers()
    .AddAuthorization();

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();