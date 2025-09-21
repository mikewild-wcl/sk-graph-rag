namespace SK.GraphRag.Application.Chunkers;

public static class TextChunker
{
    public static string[] ChunkText(
        string text, 
        int chunkSize = 1000, 
        int overlap = 200, 
        bool splitOnWhitespaceOnly = true)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(chunkSize, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(overlap, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(overlap, chunkSize);

        if (string.IsNullOrEmpty(text))
        {
            return Array.Empty<string>();
        }

        var chunks = new List<string>();
        for (int i = 0; i < text.Length; i += chunkSize - overlap)
        {
            int length = Math.Min(chunkSize, text.Length - i);
            chunks.Add(text.Substring(i, length));
            if (i + length >= text.Length) break;
        }

        /*
def chunk_text(text, chunk_size, overlap, split_on_whitespace_only=True):
    chunks = []
    index = 0

    while index < len(text):
        if split_on_whitespace_only:
            prev_whitespace = 0
            left_index = index - overlap
            while left_index >= 0:
                if text[left_index] == " ":
                    prev_whitespace = left_index
                    break
                left_index -= 1
            next_whitespace = text.find(" ", index + chunk_size)
            if next_whitespace == -1:
                next_whitespace = len(text)
            chunk = text[prev_whitespace:next_whitespace].strip()
            chunks.append(chunk)
            index = next_whitespace + 1
        else:
            start = max(0, index - overlap + 1)
            end = min(index + chunk_size + overlap, len(text))
            chunk = text[start:end].strip()
            chunks.append(chunk)
            index += chunk_size

    return chunks         */

        return chunks.ToArray();


    }

}
