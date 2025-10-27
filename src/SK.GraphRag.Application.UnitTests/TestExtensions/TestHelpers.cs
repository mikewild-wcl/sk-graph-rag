namespace SK.GraphRag.Application.UnitTests.TestExtensions;

internal static class TestHelpers
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
