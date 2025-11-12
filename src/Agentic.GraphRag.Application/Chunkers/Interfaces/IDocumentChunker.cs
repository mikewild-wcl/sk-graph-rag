namespace Agentic.GraphRag.Application.Chunkers.Interfaces;


public interface IDocumentChunker
{
    IAsyncEnumerable<string> StreamTextChunks(string filePath, CancellationToken cancellationToken = default);
}
