using Neo4j.Driver;
using SK.GraphRag.Application.Services;
using SK.GraphRag.Application.Services.Interfaces;
using SK.GraphRag.SharedConstants;

namespace SK.GraphRag.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IGraphDataService, GraphDataService>();
        services.AddScoped<IMoviesGraphQueryService, MoviesGraphQueryService>();
        services.AddScoped<IEinsteinQueryService, EinsteinQueryService>();

        return services;
    }

    internal static IServiceCollection RegisterGraphDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole());
        var buildLogger = loggerFactory.CreateLogger<Program>();

#pragma warning disable CA1848 // Use the LoggerMessage delegates
        buildLogger.LogInformation("Configuring Neo4j with Connection: {Connection}, User: {User}",
            configuration[$"{ResourceNames.GraphDatabaseSection}:{ResourceNames.Connection}"],
            configuration[$"{ResourceNames.GraphDatabaseSection}:{ResourceNames.User}"]);

        if (configuration[$"{ResourceNames.GraphDatabaseSection}:{ResourceNames.Connection}"] is null)
        {
            buildLogger.LogInformation("Neo4j connection string is not configured. It should be set up in Neo4j:Connection");
            return services; // or throw a configuration exception            
        }
#pragma warning restore CA1848 // Use the LoggerMessage delegates

        services.AddSingleton(sp =>
        {
            var connection = configuration[$"{ResourceNames.GraphDatabaseSection}:{ResourceNames.Connection}"];
            var user = configuration[$"{ResourceNames.GraphDatabaseSection}:{ResourceNames.User}"];
            var password = configuration[$"{ResourceNames.GraphDatabaseSection}:{ResourceNames.Password}"];

            return GraphDatabase.Driver(new Uri(connection!), AuthTokens.Basic(user, password));
        });

        return services;
    }
}
