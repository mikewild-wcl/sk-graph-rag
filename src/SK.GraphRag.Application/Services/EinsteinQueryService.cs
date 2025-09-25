using SK.GraphRag.Application.Services.Interfaces;

namespace SK.GraphRag.Application.Services;

public sealed class EinsteinQueryService : IEinsteinQueryService
{
    private static readonly Dictionary<string, string> _facts = new(StringComparer.OrdinalIgnoreCase)
    {
        ["when was einstein born?"] = "Albert Einstein was born on March 14, 1879.",
        ["what is einstein famous for?"] = "He is famous for the theory of relativity and the equation E = mc^2.",
        ["when did einstein win the nobel prize?"] = "He received the Nobel Prize in Physics in 1921 for his explanation of the photoelectric effect.",
        ["where was einstein born?"] = "He was born in Ulm, in the Kingdom of Württemberg in the German Empire.",
        ["when did einstein die?"] = "He died on April 18, 1955 in Princeton, New Jersey, USA.",
    };

    public Task<string> AskAsync(string question, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return Task.FromResult("Please ask a question about Albert Einstein.");
        }

        var key = question.Trim();
        if (_facts.TryGetValue(key, out var answer))
        {
            return Task.FromResult(answer);
        }

        return Task.FromResult("I don't have an answer. Try asking about birth, Nobel Prize, or famous works.");
    }
}
