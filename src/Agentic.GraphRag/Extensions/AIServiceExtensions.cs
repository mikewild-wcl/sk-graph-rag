using Agentic.GraphRag.Application.Settings;
using Agentic.GraphRag.Logging;
using Agentic.GraphRag.Shared.Configuration;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Extensions.AI;
using System.Diagnostics.CodeAnalysis;

namespace Agentic.GraphRag.Extensions;

[SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "False positive in extensions class")]
internal static class AIServiceExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        internal IHostApplicationBuilder AddAIServices()
        {
            var aiSettings = builder.Configuration.GetSection(AISettings.SectionName).Get<AISettings>() 
                ?? throw new InvalidOperationException("AI settings are not configured properly.");

            builder.Services.AddSingleton(aiSettings);

            builder.AddAIProvider(aiSettings);

            builder.AddAIAgents(aiSettings);

            builder.Services.AddHostedService<AIStartupLogger>();

            builder.AddDevUI();

            return builder;
        }

        private IHostApplicationBuilder AddAIProvider(AISettings aiSettings)
        {
            switch (aiSettings.Provider)
            {
                case AIProvider.Ollama:
                    builder.AddOllamaApiClient(aiSettings.DeploymentName)
                        .AddChatClient();
                    if (!string.IsNullOrEmpty(aiSettings.EmbeddingDeploymentName))
                    {
                        builder.AddOllamaApiClient(aiSettings.EmbeddingDeploymentName)
                            .AddEmbeddingGenerator();
                    }

                    /* Add resilience handler for Ollama API calls */
                    builder.Services.AddOllamaResilienceHandler();

                    break;

                case AIProvider.AzureOpenAI:
                    var azureClient = builder.AddAzureOpenAIClient("ai-service");
                    azureClient.AddChatClient();
                    if (!string.IsNullOrEmpty(aiSettings.EmbeddingDeploymentName))
                    {
                        azureClient.AddEmbeddingGenerator();
                    }
                    break;

                case AIProvider.GitHubModels:
                    var gitHubClient = builder.AddOpenAIClient(aiSettings.DeploymentName);
                    gitHubClient.AddChatClient();
                    if (!string.IsNullOrEmpty(aiSettings.EmbeddingDeploymentName))
                    {
                        var gitHubEmbeddingClient = builder.AddOpenAIClient(aiSettings.EmbeddingDeploymentName);
                        gitHubEmbeddingClient.AddEmbeddingGenerator();
                    }
                    break;

                case AIProvider.AzureAIFoundry:
                case AIProvider.AzureLocalFoundry:
                    var foundryClient = builder.AddAzureChatCompletionsClient(aiSettings.DeploymentName);
                    foundryClient.AddChatClient();
                    //foundryClient.AddEmbeddingGenerator(); //Not available yet
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported AI provider: {aiSettings.Provider}");
            }

            return builder;
        }

        private IHostApplicationBuilder AddAIAgents(AISettings aiSettings)
        {
            builder.Services.AddKeyedSingleton<AIAgent>(
                ServiceKeys.EinsteinAssistantAgent, 
                (sp, name) =>
                {
                    //var chatClient = sp.GetRequiredKeyedService<IChatClient>("chat-model");
                    var chatClient = sp.GetRequiredService<IChatClient>();
                    return new ChatClientAgent(
                        chatClient,
                        name: nameof(ServiceKeys.EinsteinAssistantAgent),
                        instructions: 
                        """
                        You're an expert on Albert Einstein, but can only use provided documents to respond to questions.
                        If you can't answer, respond with "I don't have an answer.Try asking about birth, Nobel Prize, or famous works."
                        If I refer to "Albert" I mean "Albert Einstein".
                        """);
                });

            builder.Services.AddKeyedSingleton<AIAgent>(
                ServiceKeys.EinsteinStepbackAgent,
                (sp, name) =>
                {
                    var chatClient = sp.GetRequiredService<IChatClient>();
                    return new ChatClientAgent(
                        chatClient,
                        name: nameof(ServiceKeys.EinsteinStepbackAgent),
                        instructions:
                        """
                        You are an expert at world knowledge. Your task is to step back
                        and paraphrase a question to a more generic step-back question, which
                        is easier to answer. ONLY output the step-back question without any surrounding text or description.
                        Here are a few examples:
                    
                        "input": "Could the members of The Police perform lawful arrests?"
                        "output": "What can the members of The Police do?"
                    
                        "input": "Bob Smith was born in what country?"
                        "output": "What is Bob Smith’s personal history?"
                        """);
                });

            return builder;
        }


    }
}
