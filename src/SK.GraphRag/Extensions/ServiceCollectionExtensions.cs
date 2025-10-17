using Microsoft.Extensions.Options;
using Neo4j.Driver;
using SK.GraphRag.Application.Services;
using SK.GraphRag.Application.Services.Interfaces;
using SK.GraphRag.Application.Settings;

namespace SK.GraphRag.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {        
        return services
            .Configure<AzureOpenAISettings>(configuration.GetSection(AzureOpenAISettings.SectionName))
            .Configure<DownloadSettings>(configuration.GetSection(DownloadSettings.SectionName))
            .Configure<EinsteinQuerySettings>(configuration.GetSection(EinsteinQuerySettings.SectionName))
            .Configure<GraphDatabaseSettings>(configuration.GetSection(GraphDatabaseSettings.SectionName));
    }

    internal static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IGraphDataService, GraphDataService>();
        services.AddScoped<IMoviesGraphQueryService, MoviesGraphQueryService>();
        services.AddScoped<IEinsteinQueryService, EinsteinQueryService>();

        services.AddHttpClient<IDownloadService, DownloadService>(client =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<DownloadSettings>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.Timeout);
            client.DefaultRequestHeaders.Add("User-Agent", "SK.GraphRag-Downloader");
        });

        return services;
    }

    internal static IServiceCollection RegisterGraphDatabase(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<GraphDatabaseSettings>>().Value;

        using var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole());
        var buildLogger = loggerFactory.CreateLogger<Program>();

#pragma warning disable CA1848 // Use the LoggerMessage delegates
        buildLogger.LogInformation("Configuring Neo4j with Connection: {Connection}, User: {User}",
            options.Connection,
            options.User);

        if (options.Connection is null)
        {
            buildLogger.LogInformation("Neo4j connection string is not configured. It should be set up in Neo4j:Connection");
            return services; // or throw a configuration exception            
        }
#pragma warning restore CA1848 // Use the LoggerMessage delegates

        services.AddSingleton(sp =>
        {
            return GraphDatabase.Driver(options.Connection, AuthTokens.Basic(options.User, options.Password));
        });

        return services;
    }
}
