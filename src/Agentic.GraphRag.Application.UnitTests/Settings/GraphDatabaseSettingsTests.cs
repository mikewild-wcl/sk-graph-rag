using Agentic.GraphRag.Application.Settings;

namespace Agentic.GraphRag.Application.UnitTests.Settings;

public class GraphDatabaseSettingsTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var connection = new Uri("bolt://localhost:7687");
        var user = "TEST_USER";
        var password = "PASSWORD";
        var defaultDb = "neo4j";
        var defaultProvider = "neo4j";
        var defaultTimeout = 30;

        // Act
        var options = new GraphDatabaseSettings(connection, user, password);

        // Assert
        options.Connection.Should().Be(connection);
        options.User.Should().Be(user);
        options.Password.Should().Be(password);
        options.Provider.Should().Be(defaultProvider);
        options.EinsteinVectorDb.Should().Be(defaultDb);
        options.MoviesDb.Should().Be(defaultDb);
        options.UfoDb.Should().Be(defaultDb);
        options.Timeout.Should().Be(defaultTimeout);
    }

    [Fact]
    public void With_ShouldSetPropertiesCorrectly()
    {
        //Arrange
        var connection = new Uri("bolt://localhost:7687");
        var user = "TEST_USER";
        var password = "PASSWORD";
        var provider = "memgraph";
        var einsteinVectorDb = "einstein";
        var moviesDb = "movies";
        var ufoDb = "ufo";
        var timeout = 100;

        var options = new GraphDatabaseSettings(
            new Uri("https://dummy.endpoint"),
            "DUMMY_USER",
            "DUMMY_PASSWORD")
        {
            Provider = provider,
            Timeout = 10
        };

        // Act
        options = options with
        {
            Connection = connection,
            User = user,
            Password = password,
            EinsteinVectorDb = einsteinVectorDb,
            MoviesDb = moviesDb,
            UfoDb = ufoDb,
            Timeout = timeout
        };

        //Assert
        options.Connection.Should().Be(connection);
        options.User.Should().Be(user);
        options.Password.Should().Be(password);
        options.Provider.Should().Be(provider);
        options.EinsteinVectorDb.Should().Be(einsteinVectorDb);
        options.MoviesDb.Should().Be(moviesDb);
        options.UfoDb.Should().Be(ufoDb);
        options.Timeout.Should().Be(timeout);
    }

    [Fact]
    public void NonConstructorProperties_CanBeSetViaInit()
    {
        // Arrange
        var options = new GraphDatabaseSettings(new Uri("http://dummy"), "user", "pwd")
        { 
            Provider = "memgraph",
            EinsteinVectorDb = "einstein",
            MoviesDb = "movies",
            UfoDb = "ufo",
            Timeout = 99 
        };

        // Assert
        options.Provider.Should().Be("memgraph");
        options.EinsteinVectorDb.Should().Be("einstein");
        options.MoviesDb.Should().Be("movies");
        options.UfoDb.Should().Be("ufo");
        options.Timeout.Should().Be(99);
    }
}
