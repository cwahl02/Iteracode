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
        app.UseCors("AllowAll");
        app.UseAuthentication();
        //app.UseAuthorization();
        app.MapEndpoints();

        return app;
    }
}