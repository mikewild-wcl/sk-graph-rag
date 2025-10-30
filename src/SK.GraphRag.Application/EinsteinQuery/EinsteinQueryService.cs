using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using SK.GraphRag.Application.Chunkers.Interfaces;
using SK.GraphRag.Application.EinsteinQuery.Interfaces;
using SK.GraphRag.Application.Services.Interfaces;
using SK.GraphRag.Application.Settings;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;

namespace SK.GraphRag.Application.EinsteinQuery;

public sealed class EinsteinQueryService(
    [FromKeyedServices(ServiceKeys.AzureOpenAIChatClient)] IChatClient chatClient,
    IEinsteinQueryDataAccess dataAccess,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    ILogger<EinsteinQueryService> logger) : IEinsteinQueryService
{
    private readonly IChatClient _chatClient = chatClient;
    private readonly IEinsteinQueryDataAccess _dataAccess = dataAccess;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator = embeddingGenerator;
    private readonly ILogger<EinsteinQueryService> _logger = logger;

    private static readonly Dictionary<string, string> _facts = new(StringComparer.OrdinalIgnoreCase)
    {
        ["when was einstein born?"] = "Albert Einstein was born on March 14, 1879.",
        ["what is einstein famous for?"] = "He is famous for the theory of relativity and the equation E = mc^2.",
        ["when did einstein win the nobel prize?"] = "He received the Nobel Prize in Physics in 1921 for his explanation of the photoelectric effect.",
        ["where was einstein born?"] = "He was born in Ulm, in the Kingdom of Wrttemberg in the German Empire.",
        ["when did einstein die?"] = "He died on April 18, 1955 in Princeton, New Jersey, USA.",
    };

    public async Task<string> Ask(string question, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return "Please ask a question about Albert Einstein.";
        }

        var userInput = question.Trim();

        var stepBackPrompt = await GenerateStepBackPrompt(userInput, cancellationToken);

        var embedding = await _embeddingGenerator.GenerateVectorAsync(userInput, cancellationToken: cancellationToken).ConfigureAwait(false);
        var searchResults = await _dataAccess.QuerySimilarRecords(embedding);

        /* Only need one of these! */
        var stepBackResponse = await GenerateQuestionResponse(stepBackPrompt, searchResults.Select(r => r.Text).ToList(), cancellationToken);
        var finalResponse = await GenerateQuestionResponse(userInput, searchResults.Select(r => r.Text).ToList(), cancellationToken);

        return finalResponse;
    }

    private async Task<string> GenerateQuestionResponse(string userInput, List<string> searchResults, CancellationToken cancellationToken)
    {
        var encodedinput = HttpUtility.HtmlEncode(userInput);

        var prompt = $"""
            Use the following documents to answer the question that will follow:
            {string.Join("\n\n---\n\n", searchResults)} 
            ---
            The question to answer using information only from the above documents: {encodedinput}
            """;

        var agent = _chatClient.CreateAIAgent(
            instructions:
                """
                You're an expert on Albert Einstein, but can only use provided documents to respond to questions.
                If you can't answer, respond with "I don't have an answer. Try asking about birth, Nobel Prize, or famous works."
                """,
            name: "EinsteinAssistant"
            );

        var response = await agent.RunAsync(prompt, null, new AgentRunOptions(), cancellationToken);
        return response.Text;
    }

    private string CreatePrompt(string question)
    {
        return "I don't have an answer. Try asking about birth, Nobel Prize, or famous works.";
    }

    private async Task<string> GenerateStepBackPrompt(string userInput, CancellationToken cancellationToken)
    {
        var agent = _chatClient.CreateAIAgent(
            instructions:
                """
                You are an expert at world knowledge. Your task is to step back
                and paraphrase a question to a more generic step-back question, which
                is easier to answer. Here are a few examples:

                "input": "Could the members of The Police perform lawful arrests?"
                "output": "What can the members of The Police do?"

                "input": "Bob Smith was born in what country?"
                "output": "What is Bob Smith’s personal history?"
                """,
            name: "StepbackAgent"
            );

        var response = await agent.RunAsync(HttpUtility.HtmlEncode(userInput), null, new AgentRunOptions(), cancellationToken);
        return response.Text;
    }
}
