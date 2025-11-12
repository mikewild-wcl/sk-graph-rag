namespace Agentic.GraphRag.Application.UnitTests.TestExtensions;

internal static class TestMocks
{
    public static async IAsyncEnumerable<T> MockAsyncEnumerable<T>(IEnumerable<T> items)
    {
        if (items is null)
        {
            yield break;
        }

        foreach (var item in items)
        {
            yield return item;
            await Task.Yield();
        }
    }
}
