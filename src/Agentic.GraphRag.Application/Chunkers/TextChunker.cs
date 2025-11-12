using System.Runtime.CompilerServices;

namespace SK.GraphRag.Application.Chunkers;

public static class TextChunker
{
    public static string[] ChunkText(
        string text,
        int chunkSize = 1000,
        int overlap = 200)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(chunkSize, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(overlap, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(overlap, chunkSize);

        if (string.IsNullOrEmpty(text))
        {
            return [];
        }

        var chunks = new List<string>();
        var index = 0;

        while (index < text.Length)
        {
            var start = Math.Max(0, index - overlap);
            var end = Math.Min(index + chunkSize + overlap, text.Length);
            var chunk = text[start..end].Trim();
            chunks.Add(chunk);
            index += chunkSize;
        }

        return [.. chunks];
    }

    public static string[] ChunkTextOnWhitespaceOnly(
        string text,
        int chunkSize = 1000,
        int overlap = 200)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(chunkSize, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(overlap, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(overlap, chunkSize);

        if (string.IsNullOrEmpty(text))
        {
            return [];
        }

        var chunks = new List<string>();
        var index = 0;

        while (index < text.Length)
        {
            var prevWhitespace = 0;
            var leftIndex = index - overlap;
            while (leftIndex >= 0)
            {
                if (char.IsWhiteSpace(text[leftIndex]))
                {
                    prevWhitespace = leftIndex;
                    break;
                }
                leftIndex--;
            }

            var nextWhitespace = FindNextWhitespace(text, index + chunkSize);
            if (nextWhitespace == -1)
            {
                nextWhitespace = text.Length;
            }

            var chunk = text[prevWhitespace..nextWhitespace].Trim();
            chunks.Add(chunk);
            index = nextWhitespace + 1;
        }

        return [.. chunks];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindNextWhitespace(string text, int startIndex)
    {
        for (int i = startIndex; i < text.Length; i++)
        {
            if (char.IsWhiteSpace(text[i]))
            {
                return i;
            }
        }
        return -1;
    }
}
