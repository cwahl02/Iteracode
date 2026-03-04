using Scalar.AspNetCore;

namespace Iteracode.Api.Extensions;

public static class ApplicationExtensions
{
    public static WebApplication UseApplicationPipeline(
        this WebApplication app)
    {
        if(app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                    options
                        .WithTitle("Interacode API Reference")
                        .WithTheme(ScalarTheme.BluePlanet)
                        .WithDefaultHttpClient(
                            ScalarTarget.CSharp,
                            ScalarClient.HttpClient
                        );
            });
        }
        
        app.UseHttpsRedirection();
        
        // TODO: Add CORS, Authentication and Authorization middlewares when needed
        app.UseCors("AllowAll");
        app.MapEndpoints();
        // app.UseAuthentication();
        // app.UseAuthorization();

        return app;
    }
}