using FluentValidation;
using Iteracode.Api.Extensions;
using Iteracode.Api.Options;
using System.Reflection;
using Iteracode.Api.Services;
using Microsoft.Extensions.Options;

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
    .AddJudge0(builder.Configuration);

builder.Services.AddHttpClient<IJudge0Service, Judge0Service>((provider, client) =>
{
    var options = provider.GetRequiredService<IOptions<Judge0Options>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);

    if (!string.IsNullOrEmpty(options.AuthToken))
        client.DefaultRequestHeaders.Add("X-Auth-Token", options.AuthToken);

});

var app = builder.Build();

app.UseApplicationPipeline();

app.Run();