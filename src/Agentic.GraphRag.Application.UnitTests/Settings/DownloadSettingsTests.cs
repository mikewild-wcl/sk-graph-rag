using Agentic.GraphRag.Application.Settings;

namespace Agentic.GraphRag.Application.UnitTests.Settings;

public class DownloadSettingsTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var downloadDirectory = "downloads";
        var defaultTimeout = 300;

        // Act
        var options = new DownloadSettings(downloadDirectory);

        // Assert
        options.DownloadDirectory.Should().Be(downloadDirectory);
        options.Timeout.Should().Be(defaultTimeout);
    }

    [Fact]
    public void With_ShouldSetPropertiesCorrectly()
    {
        //Arrange
        var downloadDirectory = "downloads";
        var timeout = 100;

        var options = new DownloadSettings("test")
        {
            Timeout = 99
        };

        // Act
        options = options with
        {
            DownloadDirectory = downloadDirectory,
            Timeout = timeout
        };

        //Assert
        options.DownloadDirectory.Should().Be(downloadDirectory);
        options.Timeout.Should().Be(timeout);
    }

    [Fact]
    public void NonConstructorProperties_CanBeSetViaInit()
    {
        // Arrange
        var options = new DownloadSettings("test")
        {
            Timeout = 99
        };

        // Assert
        options.Timeout.Should().Be(99);
    }
}
