using Scalar.AspNetCore;
using TourOperatorDataImport.API.Options;

namespace TourOperatorDataImport.API.Extensions;

public static class WebApplicationExtensions
{
    private const string ApiVersionV1 = "v1";
    private const string ApiVersionV2 = "v2";

    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        app.MapGet("/swagger/{*any}",
            () => Results.Redirect("/docs", permanent: true)
        );

        app.UseSwagger(options => { options.RouteTemplate = "docs/{documentName}/openapi.json"; });

        var apiDocOptions = app.Configuration
            .GetSection(nameof(ApiDocumentationOptions))
            .Get<ApiDocumentationOptions>()!;

        if (!app.Environment.IsProduction())
        {
            app.MapGet("/hint", (IConfiguration configuration) => Results.Ok(GetTestEncryptedHint(configuration)));
        }

        app.MapScalarApiReference("docs", options =>
        {
            options.AddDocument(ApiVersionV1, $"{apiDocOptions.ApiName} v1", "/docs/v1/openapi.json")
                .AddDocument(ApiVersionV2, $"{apiDocOptions.ApiName} v2", "/docs/v2/openapi.json");

            options.Theme = ScalarTheme.Saturn;
            options.Layout = ScalarLayout.Classic;
            options.HideClientButton = true;
            options.DefaultOpenAllTags = false;
            options.SearchHotKey = "s";

            options.WithTitle(apiDocOptions.ApiName);

            options.WithDefaultFonts(false);
        });

        return app;
    }

    private static string GetTestEncryptedHint(IConfiguration configuration)
    {
        // Implement your hint logic here
        return "Test hint for development";
    }
}