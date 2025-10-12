using SK.GraphRag.Application.Settings;

namespace SK.GraphRag.Application.UnitTests.Settings;

public class AzureOpenAiOptionsTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var apiKey = "key";
        var endpoint = "endpoint";
        var deploymentName = "deployment";
        var embeddingDeploymentName = "embedding";
        var defaultTimeout = 30;

        // Act
        var options = new AzureOpenAISettings(apiKey, endpoint, deploymentName, embeddingDeploymentName);

        // Assert
        options.ApiKey.Should().Be(apiKey);
        options.Endpoint.Should().Be(endpoint);
        options.DeploymentName.Should().Be(deploymentName);
        options.ModelId.Should().BeNull();
        options.EmbeddingDeploymentName.Should().Be(embeddingDeploymentName);
        options.EmbeddingModelId.Should().BeNull();
        options.Timeout.Should().Be(defaultTimeout);
    }

    [Fact]
    public void With_ShouldSetPropertiesCorrectly()
    {
        //Arrange
        var apiKey = "key";
        var endpoint = "endpoint";
        var deploymentName = "deployment";
        var embeddingDeploymentName = "embedding";
        var timeout = 100;

    var options = new AzureOpenAISettings(
        "DUMMY_API_KEY",
        "https://dummy.endpoint",
        "DUMMY_DEPLOYMENT",
        "DUMMY_EMBEDDING_DEPLOYMENT")
    {
        Timeout = 10
    };

    // Act
    options = options with
        {
            ApiKey = apiKey,
            Endpoint = endpoint,
            DeploymentName = deploymentName,
            EmbeddingDeploymentName = embeddingDeploymentName,
            Timeout = timeout
        };

        //Assert
        options.ApiKey.Should().Be(apiKey);
        options.Endpoint.Should().Be(endpoint);
        options.DeploymentName.Should().Be(deploymentName);
        options.EmbeddingDeploymentName.Should().Be(embeddingDeploymentName);
        options.Timeout.Should().Be(timeout); 
    }

    [Fact]
    public void Timeout_CanBeSetViaInit()
    {
        // Arrange
        var options = new AzureOpenAISettings("a", "b", "c", "d") { Timeout = 99 };

        // Assert
        options.Timeout.Should().Be(99);
    }
}
