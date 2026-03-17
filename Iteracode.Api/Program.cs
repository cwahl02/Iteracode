using FluentValidation;
using Iteracode.Api.Extensions;
using Iteracode.Api.Options;
using Iteracode.Api.Services;
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
    .AddAuthorization()
    .Configure<VfsOptions>(builder.Configuration.GetSection("Vfs"))
    .AddHostedService<RecyclePurgeService>();

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();