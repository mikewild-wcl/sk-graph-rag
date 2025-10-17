using SK.GraphRag.Application.Settings;

namespace SK.GraphRag.Application.UnitTests.Settings;

public class DownloadSettingsTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var downloadDirectory = "downloads";
        var documentFileName = "Test.pdf";
        var documentUri = new Uri("http://localhost/test");
        var defaultTimeout = 300;

        // Act
        var options = new DownloadSettings(downloadDirectory)
        {
            EinsteinDocumentFileName = documentFileName,
            EinsteinDocumentUri = documentUri
        };

        // Assert
        options.DownloadDirectory.Should().Be(downloadDirectory);
        options.EinsteinDocumentFileName.Should().Be(documentFileName);
        options.EinsteinDocumentUri.Should().Be(documentUri);
        options.Timeout.Should().Be(defaultTimeout);
    }

    [Fact]
    public void With_ShouldSetPropertiesCorrectly()
    {
        //Arrange
        var downloadDirectory = "downloads";
        var documentFileName = "Test.pdf";
        var documentUri = new Uri("http://localhost/test");
        var timeout = 100;

        var options = new DownloadSettings("test")
        {
            EinsteinDocumentFileName = "dummydoc.pdf",
            EinsteinDocumentUri = new Uri("http://localhost/dummy"),
            Timeout = 99
        };

        // Act
        options = options with
        {
            DownloadDirectory = downloadDirectory,
            EinsteinDocumentFileName = documentFileName,
            EinsteinDocumentUri = documentUri,
            Timeout = timeout
        };

        //Assert
        options.DownloadDirectory.Should().Be(downloadDirectory);
        options.EinsteinDocumentFileName.Should().Be(documentFileName);
        options.EinsteinDocumentUri.Should().Be(documentUri);
        options.Timeout.Should().Be(timeout);
    }

    [Fact]
    public void NonConstructorProperties_CanBeSetViaInit()
    {
        // Arrange
        var options = new DownloadSettings("test")
        {
            EinsteinDocumentFileName = "mydoc.pdf",
            EinsteinDocumentUri = new Uri("http://localhost/test"),
            Timeout = 99
        };

        // Assert
        options.EinsteinDocumentFileName.Should().Be("mydoc.pdf");
        options.EinsteinDocumentUri.AbsoluteUri.Should().Be("http://localhost/test");
        options.Timeout.Should().Be(99);
    }
}
