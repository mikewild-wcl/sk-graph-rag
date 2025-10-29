using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SK.GraphRag.Application.Chunkers.Interfaces;
using SK.GraphRag.Application.EinsteinQuery.Interfaces;
using SK.GraphRag.Application.Services.Interfaces;
using SK.GraphRag.Application.Settings;
using System.Globalization;
using System.Text;

namespace SK.GraphRag.Application.EinsteinQuery;

public sealed class EinsteinQueryService(
    IEinsteinQueryDataAccess dataAccess,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IOptions<EinsteinQuerySettings> queryOptions,
    ILogger<EinsteinQueryService> logger) : IEinsteinQueryService
{
    private readonly IEinsteinQueryDataAccess _dataAccess = dataAccess;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator = embeddingGenerator;
    private readonly EinsteinQuerySettings _querySettings = queryOptions.Value;
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

        var embedding = await _embeddingGenerator.GenerateVectorAsync(question.Trim(), cancellationToken: cancellationToken).ConfigureAwait(false);

        //TODO: Query Graph DB for relevant chunks
        //TODO: Call MAF chat completion

        var key = question.Trim();
        if (_facts.TryGetValue(key, out var answer))
        {
            return answer;
        }

        return "I don't have an answer. Try asking about birth, Nobel Prize, or famous works.";
    }
}
